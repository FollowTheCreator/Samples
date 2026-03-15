
// Setup/configuration.
#load "config/misc.csx"
#load "config/targets.csx"
#load "config/package-versions.csx"

if (!Options.Quiet)
    Console.WriteLine("Generating model \"{0}\"...", projectName);

List<Action> allActions = new List<Action>();

// Customized project-layout configuration.
#load "layouts.csx"

// Add actions to create or update all of the single-instance classes and files.
#load "single-actions/misc.csx"
//#load "single-actions/graphql.csx"
//#load "single-actions/runner.csx"

// Add actions to create or update all of the multi-instance classes and files.
#load "multi-actions/enums.csx"
#load "multi-actions/entities.csx"
//#load "multi-actions/views.csx"
//#load "multi-actions/types.csx"
//#load "multi-actions/commands.csx"
//#load "multi-actions/remotes.csx"
//#load "multi-actions/wrapped-types.csx"

// Run all the actions, in parallel by default.
RunActions(allActions, parallel: !Options.DebugMode);

if (!Options.Quiet)
{
    HideProgressBar();
    Console.WriteLine("Done.");
}
