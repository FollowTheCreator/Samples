//if (ProjectLayout == "gqapi") {
//    allActions.AddRange(new Action[]
//    {
//        // JSON data/configuration files.
//        () => Template.WriteFileIfNotExists("appsettings-json.cst", Path.Combine(Model.GetPath(Targets.AppSettingsJson), "appsettings.json"), new { }),
//        () => Template.WriteFileIfNotExists("filenesting.cst", ".filenesting.json", new { }),
//        () => Template.WriteFile("model-json.cst", Model.GetPathName(Targets.ModelJson, ".generated.json"), new { }),
//        () => Template.WriteFile("model-json-class.cst", Model.GetPathName(Targets.ModelJsonClass, ".generated.cs"), new { }),

//        // Resource files.
//        () => Template.WriteFile("resources-designer.cst", Model.GetPathName(Targets.Resources, ".generated.Designer.cs"), new { }),
//        () => Template.WriteFile("resources-resx.cst", Model.GetPathName(Targets.Resources, ".generated.resx"), new { }),
//    });
//}
//else {
//allActions.AddRange(new Action[]
//{
//         JSON data/configuration files.
//        () => Template.WriteFileIfNotExists("appsettings-json.cst", Path.Combine(Model.GetPath(Targets.AppSettingsJson), "appsettings.json"), new { }),
//        () => Template.WriteFileIfNotExists("filenesting.cst", ".filenesting.json", new { }),
//        () => Template.WriteFile("model-json.cst", Model.GetPathName(Targets.ModelJson, ".generated.json"), new { }),
//        () => Template.WriteFile("model-json-class.cst", Model.GetPathName(Targets.ModelJsonClass, ".generated.cs"), new { }),

//         C# files.
//        () => Template.WriteFile("dbcontext.cst", Model.GetPathName(Targets.DbContext, ".generated.cs"), new { }),
//        () => Template.WriteFileIfNotExists("dbcontext.partial.cst", Model.GetPathName(Targets.DbContext, ".cs"), new { }),
//        () => Template.WriteFile("dbcontext-factory.cst", Model.GetPathName(Targets.DbContextFactory, ".generated.cs"), new { }),
//        () => Template.WriteFile("id-converters.cst", Model.GetPathName(Targets.IdTypeValueConverters, ".generated.cs"), new { }),

//         Resource files.
//        () => Template.WriteFile("resources-designer.cst", Model.GetPathName(Targets.Resources, ".generated.Designer.cs"), new { }),
//        () => Template.WriteFile("resources-resx.cst", Model.GetPathName(Targets.Resources, ".generated.resx"), new { }),
//});
//}