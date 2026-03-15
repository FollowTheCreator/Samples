using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frameworks.ExtendedProperties.Attributes;
using Frameworks.ExtendedProperties.Services.Implementations;
using IRT.Domain.Services.Impl;
using IRT.Domain.ViewsSql.DataTransferSettings;
using IRT.Domain.ViewsSql.Study;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings.Resources;
using Kernel.Globalization.Attributes;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings
{
    [ExtendsEntity(typeof(StudySqlView), typeof(ExtendedPropertiesDataService<DataTransferSettingSqlView>))]
    [Unblinding(false)]
    public class GenericEdcDataTransferStudySettings
    {
        #region Rave

        [Display(Order = 4,
            Name = GenericEdcStudySettings.ResourceNames.EnableRaveErrorNotification,
            Description = GenericEdcStudySettings.ResourceNames.EnableRaveErrorNotificationDescription,
            ResourceType = typeof(GenericEdcStudySettings))]
        [LocalizedCategory(GenericEdcStudySettings.ResourceNames.DataTransferRaveCategory, typeof(GenericEdcStudySettings))]
        public bool EnableRaveErrorNotification { get; set; }

        #endregion

        #region Veeva

        [Display(Order = 5,
            Name = GenericEdcStudySettings.ResourceNames.EnableVeevaErrorNotification,
            Description = GenericEdcStudySettings.ResourceNames.EnableVeevaErrorNotificationDescription,
            ResourceType = typeof(GenericEdcStudySettings))]
        [LocalizedCategory(GenericEdcStudySettings.ResourceNames.DataTransferVeevaCategory, typeof(GenericEdcStudySettings))]
        public bool EnableVeevaErrorNotification { get; set; }

        #endregion

        #region Repeat Key

        [Display(Order = 6,
            Name = GenericEdcStudySettings.ResourceNames.ExcludeFromScheduledVisits,
            Description = GenericEdcStudySettings.ResourceNames.ExcludeFromScheduledVisitsDescription,
            ResourceType = typeof(GenericEdcStudySettings))]
        [LocalizedCategory(GenericEdcStudySettings.ResourceNames.DataTransferRepeatKeyCategory, typeof(GenericEdcStudySettings))]
        [EditBehavior(OptionsProviderType = typeof(ScheduledVisitsOptionsProvider))]
        [DefaultValue(new string[0])]
        [DesignOnly(true)]
        public string[] ExcludeFromScheduledVisits { get; set; }

        [Display(Order = 7,
            Name = GenericEdcStudySettings.ResourceNames.UnscheduledVisits,
            Description = GenericEdcStudySettings.ResourceNames.UnscheduledVisitsDescription,
            ResourceType = typeof(GenericEdcStudySettings))]
        [LocalizedCategory(GenericEdcStudySettings.ResourceNames.DataTransferRepeatKeyCategory, typeof(GenericEdcStudySettings))]
        [EditBehavior(OptionsProviderType = typeof(VisitsOptionsProvider))]
        [DefaultValue(new string[0])]
        [DesignOnly(true)]
        public string[] UnscheduledVisits { get; set; }

        [Display(Order = 8,
            Name = GenericEdcStudySettings.ResourceNames.ReplacementVisits,
            Description = GenericEdcStudySettings.ResourceNames.ReplacementVisitsDescription,
            ResourceType = typeof(GenericEdcStudySettings))]
        [LocalizedCategory(GenericEdcStudySettings.ResourceNames.DataTransferRepeatKeyCategory, typeof(GenericEdcStudySettings))]
        [EditBehavior(OptionsProviderType = typeof(VisitsOptionsProvider))]
        [DefaultValue(new string[0])]
        [DesignOnly(true)]
        public string[] ReplacementVisits { get; set; }

        [Display(Order = 9,
            Name = GenericEdcStudySettings.ResourceNames.ScreenFailVisits,
            Description = GenericEdcStudySettings.ResourceNames.ScreenFailVisitsDescription,
            ResourceType = typeof(GenericEdcStudySettings))]
        [LocalizedCategory(GenericEdcStudySettings.ResourceNames.DataTransferRepeatKeyCategory, typeof(GenericEdcStudySettings))]
        [EditBehavior(OptionsProviderType = typeof(VisitsOptionsProvider))]
        [DefaultValue(new string[0])]
        [DesignOnly(true)]
        public string[] ScreenFailVisits { get; set; }

        [Display(Order = 10,
            Name = GenericEdcStudySettings.ResourceNames.InformedConsentVisits,
            Description = GenericEdcStudySettings.ResourceNames.InformedConsentVisitsDescription,
            ResourceType = typeof(GenericEdcStudySettings))]
        [LocalizedCategory(GenericEdcStudySettings.ResourceNames.DataTransferRepeatKeyCategory, typeof(GenericEdcStudySettings))]
        [EditBehavior(OptionsProviderType = typeof(VisitsOptionsProvider))]
        [DefaultValue(new string[0])]
        [DesignOnly(true)]
        public string[] InformedConsentVisits { get; set; }

        #endregion
    }
}
