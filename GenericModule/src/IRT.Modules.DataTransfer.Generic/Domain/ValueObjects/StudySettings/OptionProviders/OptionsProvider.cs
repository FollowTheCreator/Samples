using System.Collections.Generic;
using System.Linq;
using Frameworks.ExtendedProperties.Metadata;
using Frameworks.ExtendedProperties.ValueObjects;
using IRT.Domain.Services.Impl;
using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;

namespace IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings.OptionProviders
{
    public class OptionsProvider<TRepository> : IrtOptionsProvider where TRepository : IServiceFactory
    {
        private readonly TRepository providerRepository;

        public OptionsProvider(TRepository providerRepository)
        {
            this.providerRepository = providerRepository;
        }

        public override IEnumerable<ExtendedPropertyOption> GetOptions(EntityInfo entityInfo, ExtendedPropertyMetadata extendedPropertyMetadata)
        {
            return providerRepository.GetServices()
                .Select(x => new ExtendedPropertyOption
                {
                    Value = x.ServiceId,
                    Text = x.DisplayName
                });
        }
    }
}