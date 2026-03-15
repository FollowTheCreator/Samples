using System;
using Frameworks.ExtendedProperties.Configuration;
using Frameworks.ExtendedProperties.Metadata;
using Frameworks.ExtendedProperties.ValueObjects;
using Kernel.DDD.Dispatching;
using Kernel.DDD.Domain;
using Kernel.DDD.Domain.Commands;
using Kernel.DDD.Domain.Events;
using Unity;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DtExtendedProperty
{
    internal class DtExtendedPropertiesCommandHandler : ICommandHandler
    {
        private readonly IExtendedPropertiesFunctionConfigurator functionConfigurator;
        private readonly IUnityContainer unityContainer;

        public DtExtendedPropertiesCommandHandler(
            IExtendedPropertiesFunctionConfigurator functionConfigurator,
            IUnityContainer unityContainer)
        {
            this.functionConfigurator = functionConfigurator;
            this.unityContainer = unityContainer;
        }

        public void Handle(UpdateCommand<DtExtendedPropertyDto> c)
        {
            var extendedProperty = c.Item;

            var entityBehavior = functionConfigurator.GetEntityBehavior(extendedProperty.EntityTypeName);
            var entityInfo = new EntityInfo
            {
                EntityTypeName = extendedProperty.EntityTypeName,
                EntityId = extendedProperty.EntityId
            };

            Perform(entityInfo, extendedProperty.ExtendedPropertiesTypeName, entityBehavior, aggregate =>
            {
                aggregate.AddDomainEvent(new UpdatedEvent<DtExtendedPropertyDto> { Item = extendedProperty });
            });
        }

        private void Perform(
            EntityInfo entityInfo,
            string extendedPropertiesTypeName,
            EntityBehavior entityBehavior,
            Action<DomainAggregate> action)
        {
            var aggregateRepositoryType = typeof(AggregateRepository<>).MakeGenericType(entityBehavior.AggregateType);
            var performMethod = aggregateRepositoryType.GetMethod("Perform", new[] { typeof(string), action.GetType() });
            var aggregateId = entityBehavior.AggregateIdFactory(entityInfo.EntityTypeName, extendedPropertiesTypeName, entityInfo.EntityId);

            var aggregateRepository = unityContainer.Resolve(aggregateRepositoryType);
            performMethod.Invoke(aggregateRepository, new object[] { aggregateId, action });
        }
    }
}
