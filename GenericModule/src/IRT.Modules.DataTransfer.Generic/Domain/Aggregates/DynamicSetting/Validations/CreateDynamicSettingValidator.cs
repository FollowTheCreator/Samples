using System.Linq;
using System.Text.RegularExpressions;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting.Commands;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.DynamicSetting;
using Kernel.DDD.Dispatching.Exceptions;
using Kernel.DDD.Domain.Validators;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting.Validations
{
    public class CreateDynamicSettingValidator : CommandValidatorBase<CreateDynamicSetting>
    {
        private readonly IQueryable<DynamicSettingSqlView> dynamicSettingsQuery;

        public CreateDynamicSettingValidator(IQueryable<DynamicSettingSqlView> dynamicSettingsQuery)
        {
            this.dynamicSettingsQuery = dynamicSettingsQuery;
        }

        protected override void Validate(CreateDynamicSetting c)
        {
            if (dynamicSettingsQuery.Any(x => x.DynamicSettingId == c.DynamicSettingId))
            {
                throw new CommandValidationException(Resources.ValidationResources.SameId);
            }

            if (string.IsNullOrWhiteSpace(c.Name))
            {
                throw new CommandValidationException(Resources.ValidationResources.EmptyName);
            }

            if (dynamicSettingsQuery.Any(x => x.Name == c.Name))
            {
                throw new CommandValidationException(Resources.ValidationResources.SameName);
            }

            if (string.IsNullOrWhiteSpace(c.EntityTypeName))
            {
                throw new CommandValidationException(Resources.ValidationResources.EmptyEntityTypeName);
            }

            if (!Regex.IsMatch(c.EntityTypeName, @"^[a-zA-Z0-9_.]+$"))
            {
                throw new CommandValidationException(Resources.ValidationResources.ValidExtendedPropertyName);
            }

            if (string.IsNullOrWhiteSpace(c.ExtendedPropertyName))
            {
                throw new CommandValidationException(Resources.ValidationResources.EmptyExtendedPropertyName);
            }

            if (dynamicSettingsQuery.Any(x => x.ExtendedPropertyName == c.ExtendedPropertyName))
            {
                throw new CommandValidationException(Resources.ValidationResources.SameExtendedPropertyName);
            }

            if (!Regex.IsMatch(c.ExtendedPropertyName, @"^[a-zA-Z0-9_]+$"))
            {
                throw new CommandValidationException(Resources.ValidationResources.ValidExtendedPropertyName);
            }
        }
    }
}
