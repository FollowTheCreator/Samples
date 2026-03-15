using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frameworks.ExtendedProperties.Attributes;
using Frameworks.ExtendedProperties.Services.Implementations;
using IRT.Domain.ViewsSql.DataTransferSettings;
using IRT.Domain.ViewsSql.Study;
using Kernel.Globalization.Attributes;

namespace IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings
{
    [ExtendsEntity(typeof(StudySqlView), typeof(ExtendedPropertiesDataService<DataTransferSettingSqlView>))]
    [LocalizedCategory(Resources.GenericStudySettings.ResourceNames.DataTransferCategory, typeof(Resources.GenericStudySettings))]
    [Unblinding(false)]
    public class GenericDataTransferStudySettings
    {
        [Display(Order = 1,
            Name = Resources.GenericStudySettings.ResourceNames.UseGenericDataTransfer,
            Description = Resources.GenericStudySettings.ResourceNames.UseGenericDataTransfer,
            ResourceType = typeof(Resources.GenericStudySettings))]
        [DesignOnly(true)]
        public bool UseGenericDataTransfer { get; set; }

        [Display(Order = 2,
            Name = Resources.GenericStudySettings.ResourceNames.Timeout,
            Description = Resources.GenericStudySettings.ResourceNames.TimeoutDescription,
            ResourceType = typeof(Resources.GenericStudySettings))]
        [DefaultValue(30)]
        [DesignOnly(true)]
        public int? Timeout { get; set; }

        [Display(Order = 3,
            Name = Resources.GenericStudySettings.ResourceNames.MaxNumberOfRetriesClientNotifications,
            Description = Resources.GenericStudySettings.ResourceNames.MaxNumberOfRetriesClientNotificationsDescription,
            ResourceType = typeof(Resources.GenericStudySettings))]
        [DefaultValue(3)]
        [DesignOnly(true)]
        public int MaxNumberOfRetriesForClientNotifications { get; set; }

        [Display(Order = 4,
            Name = Resources.GenericStudySettings.ResourceNames.MaxNumberOfRetriesApacheHealthCheck,
            Description = Resources.GenericStudySettings.ResourceNames.MaxNumberOfRetriesApacheHealthCheckDescription,
            ResourceType = typeof(Resources.GenericStudySettings))]
        [DefaultValue(3)]
        [DesignOnly(true)]
        public int MaxNumberOfRetriesForApacheHealthCheck { get; set; }
    }
}
