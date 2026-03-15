
public interface ICsProjectManager
{
    IEnumerable<Action> CreateOrUpdateCsProjects();
}

public interface ICustomActions<T>
    where T : class, INamedType
{
    IEnumerable<Action> GetCustomActions(T targetObject);
}

#load "layouts/*.csx"

IOutputNamingRules OutputNamingRules = CreateInstanceForProjectLayout<IOutputNamingRules>(
    ScriptAssembly, "OutputNamingRules", This);
ICsProjectManager CsProjectManager = CreateInstanceForProjectLayout<ICsProjectManager>(
    ScriptAssembly, "CsProjectManager", This, PackageVersions);

if (OutputNamingRules == null
    || CsProjectManager == null)
    Abort();

Model.OutputNamingRules = OutputNamingRules;
allActions.AddRange(CsProjectManager.CreateOrUpdateCsProjects());
