using System;
using System.Collections.Generic;
using System.Linq;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey
{
    public class ItemGroupRepeatKeyFilterModel
    {
        public ItemGroupRepeatKeyFilterModel()
        {
        }

        public ItemGroupRepeatKeyFilterModel(GenericEdcNotificationDefinitionSettings notificationDefinitionSettings)
        {
            RepeatKeyBasis = notificationDefinitionSettings.ItemGroupRepeatKeyBasis;

            RepeatKeyCounterBasis = notificationDefinitionSettings.ItemGroupRepeatKeyCounterBasis;

            ReuseRepeatKey = notificationDefinitionSettings.AllowItemGroupRepeatKeyReuse;

            ExistingRepeatKeys = !notificationDefinitionSettings.ExistingItemGroupRepeatKeys.IsNullOrEmpty()
                ? notificationDefinitionSettings.ExistingItemGroupRepeatKeys
                    .SplitByComma()
                    .Select(x => x.Trim())
                    .ToArray()
                :
                [];

            RepeatKeyInitialValue = notificationDefinitionSettings.ItemGroupRepeatKeyCounterInitialValue;

            UnscheduledVisitsStrategy = notificationDefinitionSettings.ItemGroupRepeatKeyUnscheduledVisitsStrategy;

            ReplacementVisitsStrategy = notificationDefinitionSettings.ItemGroupRepeatKeyReplacementVisitsStrategy;

            OverwriteReplacedDrug = notificationDefinitionSettings.ItemGroupRepeatKeyOverwriteReplacedDrug;

            ResendReplacedDrugOnBackout = notificationDefinitionSettings.ItemGroupRepeatKeyResendReplacedDrugOnBackout;

            OverwriteManuallyReplacedDrug = notificationDefinitionSettings.ItemGroupRepeatKeyOverwriteManuallyReplacedDrug;

            ResendManuallyReplacedDrugOnDelete = notificationDefinitionSettings.ItemGroupRepeatKeyResendManuallyReplacedDrugOnDelete;

            InsertTransactionType = notificationDefinitionSettings.ItemGroupRepeatKeyInsertTransactionType;

            UpdateTransactionType = notificationDefinitionSettings.ItemGroupRepeatKeyUpdateTransactionType;

            RemoveTransactionType = notificationDefinitionSettings.ItemGroupRepeatKeyRemoveTransactionType;
        }

        public Guid NotificationDefinitionId { get; set; }

        public RepeatKeyBasis RepeatKeyBasis { get; set; }

        public RepeatKeyCounterBasis RepeatKeyCounterBasis { get; set; }

        public bool ReuseRepeatKey { get; set; }

        public int RepeatKeyInitialValue { get; set; }

        public string[] ExistingRepeatKeys { get; set; }

        public UnscheduledVisitsStrategy UnscheduledVisitsStrategy { get; set; }

        public ReplacementVisitsStrategy ReplacementVisitsStrategy { get; set; }

        public bool OverwriteReplacedDrug { get; set; }

        public bool ResendReplacedDrugOnBackout { get; set; }

        public bool OverwriteManuallyReplacedDrug { get; set; }

        public bool ResendManuallyReplacedDrugOnDelete { get; set; }

        public string InsertTransactionType { get; set; }

        public string UpdateTransactionType { get; set; }

        public string RemoveTransactionType { get; set; }

        public List<DrugInfo> Drugs { get; set; }

        public List<string> ScreenFailReasons { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SubjectVisitId { get; set; }

        public Guid? ParentSubjectVisitId { get; set; }

        public string VisitId { get; set; }

        public string ParentVisitId { get; set; }

        public string SiteId { get; set; }

        public bool SelfSupport { get; set; }
    }
}
