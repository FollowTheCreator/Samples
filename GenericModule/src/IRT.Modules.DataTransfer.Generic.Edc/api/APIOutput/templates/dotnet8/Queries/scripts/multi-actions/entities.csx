
[Flags]
enum AdfCommandKind {
    Create = 1,
    Update = 2,
    Delete = 3,
    Upsert = 4,

    Batch = 16,
    Hidden = 32,

    CreateBatch = Create | Batch,
    UpdateBatch = Update | Batch,
    DeleteBatch = Delete | Batch,
    UpsertBatch = Upsert | Batch,

    HiddenCreate = Hidden | Create,
    HiddenUpdate = Hidden | Update, 
    HiddenDelete = Hidden | Delete,
    HiddenUpsert = Hidden | Upsert,

    HiddenCreateBatch = Hidden | CreateBatch,
    HiddenUpdateBatch = Hidden | UpdateBatch,
    HiddenDeleteBatch = Hidden | DeleteBatch,
    HiddenUpsertBatch = Hidden | UpsertBatch
}

var commandsRequiringArchivers = new HashSet<AdfCommandKind>
{
    AdfCommandKind.Delete,
    AdfCommandKind.DeleteBatch,
    AdfCommandKind.HiddenDelete,
    AdfCommandKind.HiddenDeleteBatch,
};

var commandsRequiringFactories = new HashSet<AdfCommandKind>
{
    AdfCommandKind.Create,
    AdfCommandKind.CreateBatch,
    AdfCommandKind.Upsert,
    AdfCommandKind.UpsertBatch,
    AdfCommandKind.HiddenCreate,
    AdfCommandKind.HiddenCreateBatch,
    AdfCommandKind.HiddenUpsert,
    AdfCommandKind.HiddenUpsertBatch
};

var commandsRequiringUpdaters = new HashSet<AdfCommandKind>
{
    AdfCommandKind.Update,
    AdfCommandKind.UpdateBatch,
    AdfCommandKind.Upsert,
    AdfCommandKind.UpsertBatch,
    AdfCommandKind.HiddenUpdate,
    AdfCommandKind.HiddenUpdateBatch,
    AdfCommandKind.HiddenUpsert,
    AdfCommandKind.HiddenUpsertBatch
};

ICustomActions<EntityInfo> entityCustomActions = TryCreateInstanceForProjectLayout<ICustomActions<EntityInfo>>(
    ScriptAssembly, "CustomActions", This);

// Write all the entities.
foreach (EntityInfo entityInfo in Model.Entities.Where(e => !e.IsExtern && !e.IsView))
{
    if (entityInfo.ShapeName.EndsWith("Enum"))
        continue;

    allActions.AddRange(WriteEntity(entityInfo));

    if (entityCustomActions != null)
        allActions.AddRange(entityCustomActions.GetCustomActions(entityInfo));
}

/// <summary>
/// Write everything related to an entity, which may change for each entity generated.
/// </summary>
IEnumerable<Action> WriteEntity(EntityInfo entityInfo)
{
    List<Action> actions = new List<Action>();

    actions.AddRange(WriteEntityClasses(entityInfo));

    if (entityInfo.Features.TryGetValue("AdfCommands", out FeatureInfo adfCommandsInfo))
    {
        actions.AddRange(WriteAdfCommandClasses(entityInfo, adfCommandsInfo));
    }

    return actions;
}

/// <summary>
/// Write the standard classes shared by all entities.
/// </summary>
IEnumerable<Action> WriteEntityClasses(EntityInfo entityInfo)
{
    return new Action[]
    {
            () => Template.WriteFile("templates/entity.cst", entityInfo.GetPathName(Targets.Entity, ".generated.cs"), new { Entity = entityInfo }),

            () => Template.WriteFileIfNotExists("templates/entity.partial.cst", entityInfo.GetPathName(Targets.Entity, ".cs"), new { Entity = entityInfo }),
    };
}

/// <summary>
/// Write the classes required to enable the use of ADF commands for this entity
/// </summary>
IEnumerable<Action> WriteAdfCommandClasses(EntityInfo entityInfo, FeatureInfo adfCommands)
{
    List<Action> adfCommandActions = new List<Action>();
    var dtosWithArchivers = new HashSet<TypeInfo>();
    var dtosWithFactories = new HashSet<TypeInfo>();
    var dtosWithUpdaters = new HashSet<TypeInfo>();
    var dtoTypeLookup = new Dictionary<AdfCommandKind, HashSet<TypeInfo>>();

    foreach(KeyValuePair<string, object> pair in adfCommands.NamedValues)
    {
        if(!Enum.TryParse<AdfCommandKind>(pair.Key, true, out var adfCommandKind))
        {
            Error(adfCommands.Location, $"AdfCommand kind '{pair.Key}' is unknown.");
            return adfCommandActions;
        }
        var dtos = pair.Value.ToString()
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .ToArray();
        foreach(var dto in dtos)
        {
            TypeInfo typeInfo = FindTypeByName(entityInfo.NameInfo, dto);

            if(!dtoTypeLookup.TryGetValue(adfCommandKind, out var dtoGroup))
                dtoTypeLookup.Add(adfCommandKind & ~AdfCommandKind.Hidden, dtoGroup = new HashSet<TypeInfo>());
            dtoGroup.Add(typeInfo);

            if(commandsRequiringArchivers.Contains(adfCommandKind))
                dtosWithArchivers.Add(typeInfo);
            if(commandsRequiringFactories.Contains(adfCommandKind))
                dtosWithFactories.Add(typeInfo);
            if(commandsRequiringUpdaters.Contains(adfCommandKind))
                dtosWithUpdaters.Add(typeInfo);
        }
    }

    foreach(var type in dtoTypeLookup.SelectMany(c => c.Value).Distinct())
        adfCommandActions.AddRange(WriteDtoCommandPartials(entityInfo, type));
    
    foreach(var dtoWithArchiver in dtosWithArchivers)
        adfCommandActions.AddRange(WriteDtoArchiver(entityInfo, dtoWithArchiver));
    foreach(var dtoWithFactory in dtosWithFactories)
        adfCommandActions.AddRange(WriteDtoFactory(entityInfo, dtoWithFactory));
    foreach(var dtoWithUpdater in dtosWithUpdaters)
        adfCommandActions.AddRange(WriteDtoUpdater(entityInfo, dtoWithUpdater));

    foreach(var singularKind in new[] {AdfCommandKind.Create, AdfCommandKind.Update, AdfCommandKind.Delete, AdfCommandKind.Upsert})
    {
        var singularDtos = dtoTypeLookup.GetValueOrDefault(singularKind, new HashSet<TypeInfo>());
        var batchDtos = dtoTypeLookup.GetValueOrDefault(singularKind | AdfCommandKind.Batch, new HashSet<TypeInfo>());
        foreach(var dtoWithValidator in singularDtos.Union(batchDtos))
        {
            adfCommandActions.AddRange(WriteCommandValidator(singularKind, entityInfo, dtoWithValidator));
        }
    }

    return adfCommandActions;
}

static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
	=> dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;

TypeInfo FindTypeByName(NameInfo startingNamespace, string nameToSearchFor)
{
    // Turn the string into a NameInfo object, splitting on any '.' found in it.
    NameInfo localName = NameInfo.FromFullName(nameToSearchFor);

    // Search up the namespace tree. This is faster than it looks, and provably correct.
    for (NameInfo currentNamespace = startingNamespace;
        currentNamespace != null;
        currentNamespace = currentNamespace.Parent)
    {
        NameInfo testName = currentNamespace.Append(localName);
        if (Model.Types.TryGet(testName, out TypeInfo typeInfo))
            return typeInfo;
    }

    // Didn't find it
    throw new Exception();
}

IEnumerable<Action> WriteDtoCommandPartials(EntityInfo entityInfo, TypeInfo typeInfo)
    => new Action[]
    {
    };

IEnumerable<Action> WriteDtoArchiver(EntityInfo entityInfo, TypeInfo typeInfo)
    => new Action[]
    {
    };

IEnumerable<Action> WriteDtoFactory(EntityInfo entityInfo, TypeInfo typeInfo)
    => new Action[]
    {
    };

IEnumerable<Action> WriteDtoUpdater(EntityInfo entityInfo, TypeInfo typeInfo)
    => new Action[]
    {
    };

IEnumerable<Action> WriteCommandValidator(AdfCommandKind kind, EntityInfo entityInfo, TypeInfo typeInfo)
    => new Action[]
    {
    };

string GetValidatorTargetKind(AdfCommandKind kind)
    => kind switch 
    {
    };