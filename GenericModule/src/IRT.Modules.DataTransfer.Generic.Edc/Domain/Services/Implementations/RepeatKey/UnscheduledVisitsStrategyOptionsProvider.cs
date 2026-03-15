using System;
using System.Collections.Generic;
using System.Linq;
using Frameworks.ExtendedProperties.Metadata;
using Frameworks.ExtendedProperties.ValueObjects;
using IRT.Domain.Services.Impl;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;
using Kernel.Utilities.Utilities;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Services.Implementations.RepeatKey
{
    public class UnscheduledVisitsStrategyOptionsProvider : IrtOptionsProvider
    {
        public override IEnumerable<ExtendedPropertyOption> GetOptions(
            EntityInfo entityInfo,
            ExtendedPropertyMetadata extendedPropertyMetadata)
        {
            var result = Enum.GetValues<UnscheduledVisitsStrategy>();

            return result
                .Select(x => new ExtendedPropertyOption
                {
                    Text = EnumHelper.GetDisplayName(typeof(UnscheduledVisitsStrategy), x) ?? x.ToString(),
                    Value = ((int)x).ToString()
                });
        }
    }
}
