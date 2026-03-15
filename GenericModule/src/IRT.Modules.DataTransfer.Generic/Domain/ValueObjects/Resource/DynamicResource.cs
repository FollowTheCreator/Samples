using System.ComponentModel.DataAnnotations;
using Frameworks.ExtendedProperties.Attributes;
using Frameworks.ExtendedProperties.Services.Implementations;
using IRT.Domain.ViewsSql.ExtendedProperty;
using IRT.Modules.DataTransfer.Generic.Domain.Resources;
using Kernel.Globalization.Entities;

namespace IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.Resource
{
    [ExtendsEntity(typeof(ResourceSqlView), typeof(ExtendedPropertiesDataService<ExtendedPropertySqlView>), shouldCacheValues: true)]
    public class DynamicResource : IRT.Domain.ValueObjects.Resources.ResourceExtendedProperties<DynamicResourceIWR>
    {
        [Display(Name = DynamicResourceColumns.ResourceNames.Name, ResourceType = typeof(DynamicResourceColumns))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = DynamicExtendedPropertiesResources.ResourceNames.DynamicResourceNameRequired, ErrorMessageResourceType = typeof(DynamicExtendedPropertiesResources))]
        [Unique(ErrorMessageResourceName = DynamicExtendedPropertiesResources.ResourceNames.DynamicResourceDuplicateName, ErrorMessageResourceType = typeof(DynamicExtendedPropertiesResources))]
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
            }
        }

        [Display(Name = DynamicResourceColumns.ResourceNames.Value, ResourceType = typeof(DynamicResourceColumns))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = DynamicExtendedPropertiesResources.ResourceNames.DynamicResourceValueRequired, ErrorMessageResourceType = typeof(DynamicExtendedPropertiesResources))]
        [Unique(ErrorMessageResourceName = DynamicExtendedPropertiesResources.ResourceNames.DynamicResourceDuplicateValue, ErrorMessageResourceType = typeof(DynamicExtendedPropertiesResources))]
        public override string Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                base.Value = value;
            }
        }

        [Display(Name = DynamicResourceColumns.ResourceNames.Comment, ResourceType = typeof(DynamicResourceColumns))]
        public override string Comment
        {
            get
            {
                return base.Comment;
            }

            set
            {
                base.Comment = value;
            }
        }
    }
}
