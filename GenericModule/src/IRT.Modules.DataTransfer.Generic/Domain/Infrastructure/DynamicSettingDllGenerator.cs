using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using Frameworks.ExtendedProperties.Attributes;
using Frameworks.ExtendedProperties.Registries;
using Frameworks.ExtendedProperties.Services.Implementations;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.DynamicDataTransferSettings;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings.Resources;
using Kernel.Globalization.Attributes;

namespace IRT.Modules.DataTransfer.Generic.Domain.Infrastructure
{
    public class DynamicSettingDllGenerator : IDynamicSettingDllGenerator
    {
        private readonly IExtendedPropertiesRegistry extendedPropertiesRegistry;

        public DynamicSettingDllGenerator(
            IExtendedPropertiesRegistry extendedPropertiesRegistry)
        {
            this.extendedPropertiesRegistry = extendedPropertiesRegistry;
        }

        public void RegisterType(TypeBuilder typeBuilder, string entityTypeName)
        {
            var extendedPropertiesType = typeBuilder.CreateType();
            var enitityType = GetType(entityTypeName);

            extendedPropertiesRegistry.RegisterExtendedProperties(enitityType, extendedPropertiesType);
        }

        private Type GetType(string entityTypeName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Single(t => t.GetType(entityTypeName) != null);
            var type = assembly.GetType(entityTypeName);
            return type;
        }

        public TypeBuilder AddType(string entityTypeName, Guid dynamicSettingId)
        {
            var enitityTypeName = Type.GetType(entityTypeName);

            var customAttributeExtendsEntity = DynamicSettingGeneratorHelper.EmitAttribute(
              typeof(ExtendsEntityAttribute),
              new Type[] { typeof(Type), typeof(Type), typeof(bool) },
              new object[] { enitityTypeName, typeof(ExtendedPropertiesDataService<DynamicDataTransferSettingSqlView>), false });

            var customAttributeLocalizedCategory = DynamicSettingGeneratorHelper.EmitAttribute(
               typeof(LocalizedCategoryAttribute),
               new Type[] { typeof(string), typeof(Type) },
               new object[] { DynamicStudySettings.ResourceNames.DataTransferDynamicSettingCategory, typeof(DynamicStudySettings) });

            var typeBuilder = DynamicSettingGeneratorHelper.EmitType(
                DynamicSettingGeneratorHelper.GetUniqueTypeName(dynamicSettingId),
                new List<CustomAttributeBuilder>() { customAttributeExtendsEntity, customAttributeLocalizedCategory });
            return typeBuilder;
        }

        public void AddProperty(TypeBuilder typeBuilder,
            string extendedPropertyName,
            string defaultValue,
            string name,
            string description)
        {
            var customAttributeDefaultValue = DynamicSettingGeneratorHelper.EmitAttribute(
               typeof(System.ComponentModel.DefaultValueAttribute),
               new Type[] { typeof(string) },
               new object[] { defaultValue });

            Type type = typeof(DisplayAttribute);

            var propertyTypes = new[]
                    {
                        type.GetProperty("Order"),
                        type.GetProperty("Name"),
                        type.GetProperty("Description")

                        //type.GetProperty("ResourceType"),
                    };
            var propertyValues = new object[]
                    {
                        1,
                        name,
                        description

                        //typeof(DynamicResourceIWR)
                    };

            var customAttributeDisplay = DynamicSettingGeneratorHelper.EmitAttribute(type, propertyTypes, propertyValues);

            var customAttributeBuilders = new List<CustomAttributeBuilder>()
            {
                customAttributeDisplay,
                customAttributeDefaultValue
            };

            DynamicSettingGeneratorHelper.EmitProperty(
                typeBuilder,
                typeof(string),
                extendedPropertyName,
                customAttributeBuilders);
        }
    }
}
