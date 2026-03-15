using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting.Commands;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting.Events;
using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting
{
    public class DynamicSettingAggregate : IRTStatelessAggregate
    {
        public void CreateDynamicSetting(
            CreateDynamicSetting command)
        {
            string description = "Extended Property FullName: "
                + DynamicSettingGeneratorHelper.GetUniqueTypeName(command.DynamicSettingId)
                + "."
                + command.Name
                + " </br>"
                + command.Description;

            AddDomainEvent(new DynamicSettingCreated
            {
                DynamicSettingId = command.DynamicSettingId,
                Name = command.Name,
                Description = description,
                DefaultValue = command.DefaultValue,
                EntityTypeName = command.EntityTypeName,
                ExtendedPropertyName = command.ExtendedPropertyName
            });

            AddDomainEvent(new DynamicSettingDllCreated
            {
                DynamicSettingId = command.DynamicSettingId,
                Name = command.Name,
                Description = description,
                DefaultValue = command.DefaultValue,
                EntityTypeName = command.EntityTypeName,
                ExtendedPropertyName = command.ExtendedPropertyName
            });
        }
    }
}
