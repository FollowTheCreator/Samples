using System;
using IrtApiCreator.ModuleSpecifics.Contracts;

namespace IRT.Modules.DataTransfer.Generic.Edc.api.ModuleSpecific
{
    public class ModuleSpecific : IrtApiCreator.ModuleSpecifics.Contracts.ModuleSpecific
        {
            public ModuleSpecific()
            {
            ExtendedPropertySpecific = new ModuleExtendedPropertySpecific();
            RightsManifestEntries = new RightsManifestEntries();
            }

            public override ILayerSpecific GetLayerSpecific(LayerEnum layer)
            {
                ILayerSpecific result;

                switch (layer)
                {
                    case LayerEnum.Commands:
                        result = new ModuleCommandSpecific();
                        break;

                    case LayerEnum.Entities:
                        result = new ModuleEntitySpecific();
                        break;

                    case LayerEnum.Events:
                        result = new ModuleEventSpecific();
                        break;

                    case LayerEnum.Queries:
                        result = new ModuleQuerySpecific();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("layer", layer.ToString());
                }

                return result;
            }

            public override string GetCommandSecurityText()
            {
                string result = GetDeclarationBody(RightsManifestEntries.Declaration);

                return result;
            }

            public override IExtendedPropertySpecific ExtendedPropertySpecific { get; }
        }
}
