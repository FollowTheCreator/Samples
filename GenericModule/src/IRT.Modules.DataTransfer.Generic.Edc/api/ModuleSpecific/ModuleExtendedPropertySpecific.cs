using IrtApiCreator.ModuleSpecifics.Contracts;

namespace IRT.Modules.DataTransfer.Generic.Edc.api.ModuleSpecific
{
    public class ModuleExtendedPropertySpecific : IExtendedPropertySpecific
    {
        public string ExtendedPropertyNamespace => "Api.Irt.Modules.DataTransfer.Generic.ExtendedProperties";

        public string OperationNamespace => "IRT.Modules.DataTransfer.Generic.Edc.Domain";

        public bool IsProject => false;

        public string ProjectReferences => string.Empty;
    }
}