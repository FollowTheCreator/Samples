
/// <summary>
/// The known kinds of output targets.  This is implemented as a class of
/// strings -- not as an enum! -- so that it can be extended on-the-fly with new things
/// if a set of templates needs to produce output that's not otherwise known.
/// 
/// The only targets that the Slang core explicitly uses are the specific names
/// "EntityId", and the base form of each construct:  i.e., "Command", "Entity", "Enum",
/// "Type", etc.  All other target objects are defined by the templates themselves.
/// </summary>
public static class Targets
{
    // Things that can be generated from a CommandInfo.
    public const string Command = "Command";
    public const string CommandHandler = "CommandHandler";
    public const string CommandValidator = "CommandValidator";
    
    // Things that can be generated from an EntityInfo (IsView = false).
    public const string Entity = "Entity";
    public const string EntityId = "EntityId";
    public const string EntityImmutable = "EntityImmutable";
    public const string EntityTypeConfig = "EntityTypeConfig";
    public const string EntityRepository = "EntityRepository";
    public const string EntityRepositoryInterface = "EntityRepositoryInterface";
    public const string EntityReadOnlyRepository = "EntityReadOnlyRepository";
    public const string EntityReadOnlyRepositoryInterface = "EntityReadOnlyRepositoryInterface";

    // Things that can be generated from an EntityInfo (IsView = true).
    public const string View = "View";
    public const string ViewHandler = "ViewHandler";

    // Things that can be generated from an EnumInfo.
    public const string Enum = "Enum";

    // Things that can be generated from a RemoteInfo.
    public const string Remote = "Remote";

    // Things that can be generated from a TypeInfo.
    public const string Type = "Type";

    // Things that can be generated from a WrappedTypeInfo.
    public const string WrappedType = "WrappedType";

    // Miscellaneous top-level things (generated from the Model).
    public const string SolutionFile = "SolutionFile";
    public const string CsProjFile = "CsProjFile";
    public const string AppSettingsJson = "AppSettingsJson";
    public const string FileNestingJson = "FileNestingJson";
    public const string ModelJson = "ModelJson";
    public const string ModelJsonClass = "ModelJsonClass";
    public const string DbContext = "DbContext";
    public const string DbContextFactory = "DbContextFactory";
    public const string IdTypeValueConverters = "IdTypeValueConverters";
    public const string Resources = "Resources";

    // Things that belong to the runner (generated from the Model).
    public const string RunnerCsProjFile = "RunnerCsProjFile";
    public const string RunnerProgram = "RunnerProgram";
    public const string RunnerStartup = "RunnerStartup";
    public const string RunnerAppSettingsJson = "RunnerAppSettingsJson";
    public const string RunnerLaunchSettingsJson = "RunnerLaunchSettingsJson";

    // Many kinds of things can generate these kinds of GraphQL output.
    public const string GraphQLType = "GraphQLType";
    public const string GraphQLInputType = "GraphQLInputType";
    public const string GraphQLCollectionResult = "GraphQLCollectionResult";
    public const string GraphQLCollectionResultType = "GraphQLCollectionResultType";
    public const string GraphQLEntityIdType = "GraphQLEntityIdType";

    // GraphQL top-level classes (generated from the Model).
    public const string GraphQLSchema = "GraphQLSchema";
    public const string GraphQLQuery = "GraphQLQuery";
    public const string GraphQLNavigation = "GraphQLNavigation";
    public const string GraphQLMutation = "GraphQLMutation";

    // FS-style solutions use multiple project files.
    public const string ApiHostCsProjFile = "ApiHostCsProjFile";
    public const string ContractsCsProjFile = "ContractsCsProjFile";
    public const string DomainCsProjFile = "DomainCsProjFile";
    public const string InfrastructureCsProjFile = "InfrastructureCsProjFile";
    public const string TestsCsProjFile = "TestsCsProjFile";

    // Classes supporting ADF commands
    public const string AdfEntityArchiver = "AdfEntityArchiver";
    public const string AdfEntityFactory = "AdfEntityFactory";
    public const string AdfEntityUpdater = "AdfEntityUpdater";
    public const string AdfCreateCommandValidator = "AdfCreateCommandValidator";
    public const string AdfUpdateCommandValidator = "AdfUpdateCommandValidator";
    public const string AdfDeleteCommandValidator = "AdfDeleteCommandValidator";
    public const string AdfUpsertCommandValidator = "AdfUpsertCommandValidator";

    public const string ServiceCollectionExtensions = "ServiceCollectionExtensions";
    public const string InitializationService = "InitializationService";

    // Classes for ADF Queries
    public const string CollectionQuery = "CollectionQuery";
    public const string SingleByIdQuery = "SingleByIdQuery";
    public const string SingleByFilterQuery = "SingleByFilterQuery";

    public const string Controller = "Controller";
}

// Tell the Slang core that we want to use the above-defined targets in the templates.
TargetsType = typeof(Targets);
