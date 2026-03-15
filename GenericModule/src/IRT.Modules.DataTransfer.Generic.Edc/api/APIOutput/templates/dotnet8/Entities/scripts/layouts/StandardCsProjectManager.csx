
/// <summary>
/// A C# project creator/updater for the 'Standard' lightweight microservice-style
/// project layout.
/// </summary>
public class StandardCsProjectManager : ICsProjectManager
{
    private ScriptGlobals Globals { get; }
    private Model Model { get; }
    private string DefaultOutputPath { get; }
    private IReadOnlyDictionary<string, string> PackageVersions { get; }
    private Template Template => Globals.Template;

    private string ProjectName => Model.TopLevelNamespace;

    // Nuget packages required by both generated .csproj files.
    private static string[] _sharedPackages =
    {
    };

    // Additional packages required by the primary generated .csproj file.
    private static string[] _mainPackages =
    {
    };

    // Additional packages required only by the Runner .csproj file.
    private static string[] _runnerPackages =
    {
        "GraphQL.Server.Transports.AspNetCore",
        "GraphQL.Server.Ui.Playground",
        "GraphQL.Hosting",
        "Unity.Microsoft.DependencyInjection",
    };

    public StandardCsProjectManager(ScriptGlobals globals, IReadOnlyDictionary<string, string> packageVersions)
    {
        Globals = globals;
        Model = globals.Model;
        DefaultOutputPath = globals.DefaultOutputPath;
        PackageVersions = packageVersions;
    }

    /// <summary>
    /// Do the actual work of maintaining the C# projects.
    /// </summary>
    public virtual IEnumerable<Action> CreateOrUpdateCsProjects()
    {
        // Update the solution and the two project files.
        return new Action[]
        {
            //() => CreateOrUpdateModelCsProj(Path.Combine(DefaultOutputPath, Model.GetPath(Targets.CsProjFile), $"{ProjectName}.csproj")),
        };
    }

    private string CalculateProjectRelativePath(string projectFile, string modelRelativePath)
        => Path.GetRelativePath(
            Path.GetFullPath(Path.GetDirectoryName(projectFile)),
            Path.GetFullPath(Path.Combine(DefaultOutputPath, modelRelativePath))
        );

    /// <summary>
    /// Programmatically create or update the Model's .csproj file to include
    /// required functionality and appropriate package versions, respecting any manual
    /// changes that may have been made to it by the programmer (as best we can).
    /// </summary>
    private void CreateOrUpdateModelCsProj(string filename)
    {
        using (CsProj csproj = new CsProj(filename))
        {
            // This is a standard library DLL.
            csproj.Project.SetAttributeValue("Sdk", "Microsoft.NET.Sdk");

            csproj.Properties["TargetFramework"] = "4.5";
            csproj.Properties["LangVersion"] = "latest";
            csproj.Properties["Nullable"] = "enable";
            csproj.Properties["WarningsAsErrors"] ??= "CS8600;CS8602;CS8603";

            // Make sure the Model doesn't try to compile the Runner by excluding its files from the build.
            if (!csproj.Properties.TryGetValue("DefaultItemExcludes", out string defaultItemExcludes))
                defaultItemExcludes = "$(DefaultItemExcludes)";
            List<string> splitExcludes = defaultItemExcludes.Split(';').DistinctInOrder().ToList();
            if (!splitExcludes.Contains("Runner\\**\\*.*"))
                splitExcludes.Add("Runner\\**\\*.*");
            csproj.Properties["DefaultItemExcludes"] = string.Join(";", splitExcludes);

            // Add the <ProjectCapabilities> for file nesting.
            csproj.ProjectCapabilities.Add("DynamicDependentFile");
            csproj.ProjectCapabilities.Add("DynamicFileNesting");

            // Ensure that we have the required packages.
            csproj.PackageReferences.AddRange(_sharedPackages, PackageVersions);
            csproj.PackageReferences.AddRange(_mainPackages, PackageVersions);
        }
    }

    /// <summary>
    /// Programmatically create or update the Runner's .csproj file to include
    /// required functionality and appropriate package versions, respecting any manual
    /// changes that may have been made to it by the programmer (as best we can).
    /// </summary>
    private void CreateOrUpdateRunnerCsProj(string filename)
    {
        using (CsProj csproj = new CsProj(filename))
        {
            // This is an ASP.NET Core web project.
            csproj.Project.SetAttributeValue("Sdk", "Microsoft.NET.Sdk.Web");

            csproj.Properties["TargetFramework"] = "netcoreapp3.1";
            csproj.Properties["LangVersion"] = "latest";
            csproj.Properties["Nullable"] = "enable";
            csproj.Properties["WarningsAsErrors"] ??= "CS8600;CS8602;CS8603";

            // Ensure that we have the required packages.
            csproj.PackageReferences.AddRange(_sharedPackages, PackageVersions);
            csproj.PackageReferences.AddRange(_runnerPackages, PackageVersions);

            // Ensure we have a project reference to the Model project.
            csproj.ProjectReferences.Add("..\\" + ProjectName + ".csproj");
        }
    }
}
