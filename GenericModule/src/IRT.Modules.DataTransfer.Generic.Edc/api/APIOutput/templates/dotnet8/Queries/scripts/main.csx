
// Setup/configuration.
#load "config/misc.csx"
#load "config/targets.csx"
#load "config/package-versions.csx"

if (!Options.Quiet)
    Console.WriteLine("Generating model \"{0}\"...", projectName);

List<Action> allActions = new List<Action>();

// Customized project-layout configuration.
#load "layouts.csx"

// Add actions to create or update all of the multi-instance classes and files.
#load "multi-actions/enums.csx"
#load "multi-actions/entities.csx"

// Run all the actions, in parallel by default.
RunActions(allActions, parallel: !Options.DebugMode);

if (!Options.Quiet)
{
    HideProgressBar();
    Console.WriteLine("Done.");
}
