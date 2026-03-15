using System;
using IRT.Domain;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting.Events
{
    public class DynamicSettingCreated : IRTEvent
    {
        public Guid DynamicSettingId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DefaultValue { get; set; }

        public string EntityTypeName { get; set; }

        public string ExtendedPropertyName { get; set; }
    }
}
