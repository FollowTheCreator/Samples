
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
        //"GraphQL",
        //"Microsoft.EntityFrameworkCore",
        //"Microsoft.AspNetCore.Mvc.NewtonsoftJson",
        //"Newtonsoft.Json",
        //"DevelopmentFramework",
        //"Filtering",
        //"GraphQL.Schema",
        //"Platform.AuthLib",
    };

    // Additional packages required by the primary generated .csproj file.
    private static string[] _mainPackages =
    {
        //"EFCore.BulkExtensions",
        //"Microsoft.EntityFrameworkCore.Design",
        //"Microsoft.EntityFrameworkCore.Relational",
        //"Microsoft.EntityFrameworkCore.SqlServer",
        //"Jint",
        //"Platform.AuditTrail.Models",
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

            // Add the <References> for dependencies.
            // csproj.References.Add("EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            // csproj.References.Add("Kernel.Audit, Version=2.3.0.0, Culture=neutral, PublicKeyToken=null");
            // csproj.References.Add("System");
            // csproj.References.Add("System.Core");
            // csproj.References.Add("System.Net.Http.Formatting, Version=5.2.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            // csproj.References.Add("System.Web.Http");
            // csproj.References.Add("System.Xml.Linq");
            // csproj.References.Add("System.Data.DataSetExtensions");
            // csproj.References.Add("Microsoft.CSharp");
            // csproj.References.Add("System.Data");
            // csproj.References.Add("System.Net.Http");
            // csproj.References.Add("System.Xml");

            // Ensure that we have the required packages.
            csproj.PackageReferences.AddRange(_sharedPackages, PackageVersions);
            csproj.PackageReferences.AddRange(_mainPackages, PackageVersions);

            // We want to include the generated .json file as an embedded resource.
            //string modelJson = CalculateProjectRelativePath(filename,
            //    Model.GetPathName(Targets.ModelJson, ".generated.json"));
            //csproj.Nones.Add(modelJson);
            //csproj.EmbeddedResources.Add(modelJson);

            // Include the string table as an embedded resource.
            //string resources = CalculateProjectRelativePath(filename,
            //    Model.GetPathName(Targets.Resources, ".generated"));
            //csproj.Compiles.Add($"{resources}.Designer.cs").ReplaceWith(XElement.Parse($@"
            //    <Compile Update=""{resources}.Designer.cs"">
            //        <DesignTime>True</DesignTime>
            //        <AutoGen>True</AutoGen>
            //        <DependentUpon>{resources}.resx</DependentUpon>
            //    </Compile>
            //"));
            //csproj.EmbeddedResources.Add($"{resources}.resx").ReplaceWith(XElement.Parse($@"
            //    <EmbeddedResource Update=""{resources}.resx"">
            //        <Generator>ResXFileCodeGenerator</Generator>
            //        <LastGenOutput>{resources}.Designer.cs</LastGenOutput>
            //    </EmbeddedResource>
            //"));
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
