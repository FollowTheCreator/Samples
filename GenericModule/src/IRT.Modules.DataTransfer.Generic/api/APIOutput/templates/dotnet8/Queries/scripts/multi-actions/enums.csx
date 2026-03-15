
ICustomActions<EnumInfo> enumCustomActions = TryCreateInstanceForProjectLayout<ICustomActions<EnumInfo>>(
    ScriptAssembly, "CustomActions", This);

// Generate all the enums.
foreach (EnumInfo enumInfo in Model.Enums.Where(e => !e.IsExtern))
{
    if (enumInfo.IsUnreferenced)
        continue;

    allActions.AddRange(WriteEnum(enumInfo));

    if (enumCustomActions != null)
        allActions.AddRange(enumCustomActions.GetCustomActions(enumInfo));
}

/// <summary>
/// Emit the required files for an enum, which consists of its generated C# `enum`
/// declaration and its GraphQL type definition.
/// </summary>
IEnumerable<Action> WriteEnum(EnumInfo enumInfo)
    => new Action[]
    {
    };

