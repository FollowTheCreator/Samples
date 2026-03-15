
// Miscellaneous configuration variables.
string projectName = Model.TopLevelNamespace;

// These variables will be exposed to all templates.
TemplateVariables["Model"] = Model;
TemplateVariables["IsMultiTenant"] = Model.Features.ContainsKey("MultiTenant");
