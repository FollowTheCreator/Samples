using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frameworks.ExtendedProperties.Attributes;
using Frameworks.ExtendedProperties.Services.Implementations;
using Frameworks.Notifications.Entities;
using IRT.Domain.Services.Impl;
using IRT.Domain.ViewsSql.DataTransferSettings;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Services.Implementations.RepeatKey;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;
using Kernel.Globalization.Attributes;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings
{
    [ExtendsEntity(typeof(NotificationDefinitionSqlView), typeof(ExtendedPropertiesDataService<DataTransferSettingSqlView>))]
    [Unblinding(false)]
    [DesignOnly(true)]
    public class GenericEdcNotificationDefinitionSettings
    {
        #region Form Repeat Key

        [Display(Order = 1,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.FormRepeatKeysEnabled,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.FormRepeatKeysEnabledDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferFormRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        public bool FormRepeatKeysEnabled { get; set; }

        [Display(Order = 2,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.FormRepeatKeyFormat,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.FormRepeatKeyFormatDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferFormRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        public string FormRepeatKeyFormat { get; set; }

        #endregion

        #region StudyEvent Repeat Key

        [Display(Order = 1,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.StudyEventRepeatKeysEnabled,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.StudyEventRepeatKeysEnabledDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferStudyEventRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DesignOnly(true)]
        public bool StudyEventRepeatKeysEnabled { get; set; }

        [Display(Order = 2,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.StudyEventRepeatKeyFormat,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.StudyEventRepeatKeyFormatDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferStudyEventRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue("{0}")]
        public string StudyEventRepeatKeyFormat { get; set; }

        [Display(Order = 3,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.FirstStudyEventRepeatKeyFormat,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.FirstStudyEventRepeatKeyFormatDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferStudyEventRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        public string FirstStudyEventRepeatKeyFormat { get; set; }

        [Display(Order = 4,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.AllowStudyEventRepeatKeyReuse,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.AllowStudyEventRepeatKeyReuseDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferStudyEventRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue(false)]
        public bool AllowStudyEventRepeatKeyReuse { get; set; }

        [Display(Order = 5,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.StudyEventRepeatKeyBasis,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.StudyEventRepeatKeyBasisDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferStudyEventRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [EditBehavior(OptionsProviderType = typeof(RepeatKeyBasisOptionsProvider))]
        [DefaultValue(RepeatKeyBasis.Subject)]
        public RepeatKeyBasis StudyEventRepeatKeyBasis { get; set; }

        [Display(Order = 6,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.StudyEventRepeatKeyCounterInitialValue,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.StudyEventRepeatKeyCounterInitialValueDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferStudyEventRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue(0)]
        public int StudyEventRepeatKeyCounterInitialValue { get; set; }

        #endregion

        #region ItemGroup Repeat Key

        [Display(Order = 1,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeysEnabled,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeysEnabledDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DesignOnly(true)]
        public bool ItemGroupRepeatKeysEnabled { get; set; }

        [Display(Order = 2,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyFormat,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyFormatDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue("{0}")]
        public string ItemGroupRepeatKeyFormat { get; set; }

        //TODO implement setting "process manual Drug Replacements"
        [Display(Order = 3,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.FirstItemGroupRepeatKeyFormat,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.FirstItemGroupRepeatKeyFormatDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        public string FirstItemGroupRepeatKeyFormat { get; set; }

        [Display(Order = 4,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.AllowItemGroupRepeatKeyReuse,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.AllowItemGroupRepeatKeyReuseDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue(false)]
        public bool AllowItemGroupRepeatKeyReuse { get; set; }

        [Display(Order = 5,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyCounterInitialValue,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyCounterInitialValueDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue(0)]
        public int ItemGroupRepeatKeyCounterInitialValue { get; set; }

        [Display(Order = 6,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyInsertTransactionType,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyInsertTransactionTypeDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue("Insert")]
        public string ItemGroupRepeatKeyInsertTransactionType { get; set; }

        [Display(Order = 7,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyUpdateTransactionType,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyUpdateTransactionTypeDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue("Update")]
        public string ItemGroupRepeatKeyUpdateTransactionType { get; set; }

        [Display(Order = 8,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyRemoveTransactionType,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyRemoveTransactionTypeDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue("Remove")]
        public string ItemGroupRepeatKeyRemoveTransactionType { get; set; }

        [Display(Order = 9,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ExistingItemGroupRepeatKeys,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ExistingItemGroupRepeatKeysDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        public string ExistingItemGroupRepeatKeys { get; set; }

        [Display(Order = 10,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyCountSkippedVisits,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyCountSkippedVisitsDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue(true)]
        public bool ItemGroupRepeatKeyCountSkippedVisits { get; set; }

        [Display(Order = 11,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeysDrugCodesToExclude,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeysDrugCodesToExcludeDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [EditBehavior(OptionsProviderType = typeof(DrugCodeOptionsProvider))]
        [DefaultValue(new string[0])]
        public string[] ItemGroupRepeatKeysDrugCodesToExclude { get; set; }

        [Display(Order = 12,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeysVisitsToInclude,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeysVisitsToIncludeDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [EditBehavior(OptionsProviderType = typeof(VisitsOptionsProvider))]
        [DefaultValue(new string[0])]
        public string[] ItemGroupRepeatKeysVisitsToInclude { get; set; }

        [Display(Order = 13,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeysVisitsToExclude,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeysVisitsToExcludeDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [EditBehavior(OptionsProviderType = typeof(VisitsOptionsProvider))]
        [DefaultValue(new string[0])]
        public string[] ItemGroupRepeatKeysVisitsToExclude { get; set; }

        [Display(Order = 14,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyBasis,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyBasisDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [EditBehavior(OptionsProviderType = typeof(RepeatKeyBasisOptionsProvider))]
        [DefaultValue(RepeatKeyBasis.SubjectVisit)]
        public RepeatKeyBasis ItemGroupRepeatKeyBasis { get; set; }

        [Display(Order = 15,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyCounterBasis,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyCounterBasisDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [EditBehavior(OptionsProviderType = typeof(RepeatKeyCounterBasisOptionsProvider))]
        [DefaultValue(RepeatKeyCounterBasis.DrugUnitId)]
        public RepeatKeyCounterBasis ItemGroupRepeatKeyCounterBasis { get; set; }

        [Display(Order = 16,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyUnscheduledVisitsStrategy,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyUnscheduledVisitsStrategyDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [EditBehavior(OptionsProviderType = typeof(UnscheduledVisitsStrategyOptionsProvider))]
        [DefaultValue(UnscheduledVisitsStrategy.Independent)]
        public UnscheduledVisitsStrategy ItemGroupRepeatKeyUnscheduledVisitsStrategy { get; set; }

        [Display(Order = 17,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyReplacementVisitsStrategy,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyReplacementVisitsStrategyDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [EditBehavior(OptionsProviderType = typeof(ReplacementVisitsStrategyOptionsProvider))]
        [DefaultValue(ReplacementVisitsStrategy.Independent)]
        public ReplacementVisitsStrategy ItemGroupRepeatKeyReplacementVisitsStrategy { get; set; }

        // applicable only to Replacement visits
        [Display(Order = 18,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyOverwriteReplacedDrug,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyOverwriteReplacedDrugDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue(false)]
        public bool ItemGroupRepeatKeyOverwriteReplacedDrug { get; set; }

        // applicable only to Replacement visits
        [Display(Order = 19,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyResendReplacedDrugOnBackout,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyResendReplacedDrugOnBackoutDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue(false)]
        public bool ItemGroupRepeatKeyResendReplacedDrugOnBackout { get; set; }

        // applicable to all visits
        [Display(Order = 20,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyOverwriteManuallyReplacedDrug,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyOverwriteManuallyReplacedDrugDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue(false)]
        public bool ItemGroupRepeatKeyOverwriteManuallyReplacedDrug { get; set; }

        // applicable to all visits
        [Display(Order = 21,
            Name = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyResendManuallyReplacedDrugOnDelete,
            Description = Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.ItemGroupRepeatKeyResendManuallyReplacedDrugOnDeleteDescription,
            ResourceType = typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [LocalizedCategory(Resources.GenericEdcNotificationDefinitionSettings.ResourceNames.DataTransferItemGroupRepeatKeyCategory, typeof(Resources.GenericEdcNotificationDefinitionSettings))]
        [DefaultValue(false)]
        public bool ItemGroupRepeatKeyResendManuallyReplacedDrugOnDelete { get; set; }

        #endregion
    }
}
