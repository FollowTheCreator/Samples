using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frameworks.ExtendedProperties.Attributes;
using Frameworks.ExtendedProperties.Services.Implementations;
using IRT.Domain.ViewsSql.DataTransferSettings;
using IRT.Domain.ViewsSql.Visit;
using Kernel.Globalization.Attributes;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings
{
    [ExtendsEntity(typeof(VisitSqlView), typeof(ExtendedPropertiesDataService<DataTransferSettingSqlView>))]
    [Unblinding(false)]
    [DesignOnly(true)]
    public class GenericEdcVisitSettings
    {
        #region Visit Mapping

        [Display(Order = 1,
            Name = Resources.GenericEdcVisitSettings.ResourceNames.EDCVisitNameMapping,
            Description = Resources.GenericEdcVisitSettings.ResourceNames.EDCVisitNameMappingDescription,
            ResourceType = typeof(Resources.GenericEdcVisitSettings))]
        [LocalizedCategory(Resources.GenericEdcVisitSettings.ResourceNames.EDCVisitNameMappingCategory, typeof(Resources.GenericEdcVisitSettings))]
        public string VisitMapping { get; set; }

        #endregion

        #region Form Repeat Key

        [Display(Order = 1,
            Name = Resources.GenericEdcVisitSettings.ResourceNames.FormRepeatKeyFormat,
            Description = Resources.GenericEdcVisitSettings.ResourceNames.FormRepeatKeyFormatDescription,
            ResourceType = typeof(Resources.GenericEdcVisitSettings))]
        [LocalizedCategory(Resources.GenericEdcVisitSettings.ResourceNames.DataTransferFormRepeatKeyCategory, typeof(Resources.GenericEdcVisitSettings))]
        public string FormRepeatKeyFormat { get; set; }

        #endregion

        #region StudyEvent Repeat Key

        [Display(Order = 1,
            Name = Resources.GenericEdcVisitSettings.ResourceNames.StudyEventRepeatKeyFormat,
            Description = Resources.GenericEdcVisitSettings.ResourceNames.StudyEventRepeatKeyFormatDescription,
            ResourceType = typeof(Resources.GenericEdcVisitSettings))]
        [LocalizedCategory(Resources.GenericEdcVisitSettings.ResourceNames.DataTransferStudyEventRepeatKeyCategory, typeof(Resources.GenericEdcVisitSettings))]
        public string StudyEventRepeatKeyFormat { get; set; }

        [Display(Order = 2,
            Name = Resources.GenericEdcVisitSettings.ResourceNames.FirstStudyEventRepeatKeyFormat,
            Description = Resources.GenericEdcVisitSettings.ResourceNames.FirstStudyEventRepeatKeyFormatDescription,
            ResourceType = typeof(Resources.GenericEdcVisitSettings))]
        [LocalizedCategory(Resources.GenericEdcVisitSettings.ResourceNames.DataTransferStudyEventRepeatKeyCategory, typeof(Resources.GenericEdcVisitSettings))]
        public string FirstStudyEventRepeatKeyFormat { get; set; }

        #endregion

        #region ItemGroup Repeat Key

        [Display(Order = 1,
            Name = Resources.GenericEdcVisitSettings.ResourceNames.ItemGroupRepeatKeyFormat,
            Description = Resources.GenericEdcVisitSettings.ResourceNames.ItemGroupRepeatKeyFormatDescription,
            ResourceType = typeof(Resources.GenericEdcVisitSettings))]
        [LocalizedCategory(Resources.GenericEdcVisitSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcVisitSettings))]
        public string ItemGroupRepeatKeyFormat { get; set; }

        [Display(Order = 2,
            Name = Resources.GenericEdcVisitSettings.ResourceNames.FirstItemGroupRepeatKeyFormat,
            Description = Resources.GenericEdcVisitSettings.ResourceNames.FirstItemGroupRepeatKeyFormatDescription,
            ResourceType = typeof(Resources.GenericEdcVisitSettings))]
        [LocalizedCategory(Resources.GenericEdcVisitSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcVisitSettings))]
        public string FirstItemGroupRepeatKeyFormat { get; set; }

        #endregion
    }
}
