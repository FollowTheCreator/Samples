using IrtApiCreator.ModuleSpecifics.Contracts;

namespace IRT.Modules.DataTransfer.Generic.Edc.api.ModuleSpecific
{
    public class ModuleLayerSpecific : LayerSpecific
        {
            public override (string aggregate, string[] path) GetAggregateAndPath(string entityNamespace)
            {
                (string aggregate, string[] path) result = (default, default);

                result.aggregate = "DTGeneric";

                result.path = entityNamespace.Split('.');

                return result;
            }

            public const string DllNamespace = "Modules.DataTransfer.Generic";
        }
}
