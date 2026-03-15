using System;
using Kernel.DDD.Domain.Commands;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting.Commands
{
    public class CreateDynamicSetting : Command
    {
        public Guid DynamicSettingId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DefaultValue { get; set; }

        public string EntityTypeName { get; set; }

        public string ExtendedPropertyName { get; set; }
    }
}
