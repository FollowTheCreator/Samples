using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;
using Kernel.EntityFramework.Interfaces;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup
{
    [Table("GenericItemGroupRepeatKeys")]
    public class GenericItemGroupRepeatKeySqlView : IDbModuleEntity
    {
        public GenericItemGroupRepeatKeySqlView()
        {
        }

        public GenericItemGroupRepeatKeySqlView(
            int repeatKey,
            ItemGroupRepeatKeyFilterModel repeatKeyFilter,
            string transactionType = null)
        {
            RepeatKeyId = Guid.NewGuid();

            NotificationDefinitionId = repeatKeyFilter.NotificationDefinitionId;

            SubjectId = repeatKeyFilter.SubjectId;

            SubjectVisitId = repeatKeyFilter.SubjectVisitId;

            LogicalSubjectVisitId = repeatKeyFilter.SubjectVisitId;

            VisitId = repeatKeyFilter.VisitId;

            LogicalVisitId = repeatKeyFilter.VisitId;

            SiteId = repeatKeyFilter.SiteId;

            RepeatKey = repeatKey;

            TransactionType = !transactionType.IsNullOrEmpty()
                ? transactionType
                : repeatKeyFilter.ExistingRepeatKeys.Contains(repeatKey.ToString())
                    ? repeatKeyFilter.UpdateTransactionType
                    : repeatKeyFilter.InsertTransactionType;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid RepeatKeyId { get; set; }

        public Guid NotificationDefinitionId { get; set; }

        public string SiteId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SubjectVisitId { get; set; }

        // depending on the Unscheduled and Replacememnt visits strategies
        // can contain SubjectVisitId of the ReplacedDrugUnits chain or ParentSubjectVisitId
        public Guid? LogicalSubjectVisitId { get; set; }

        public string VisitId { get; set; }

        // depending on the Unscheduled and Replacememnt visits strategies
        // can contain VisitId of the ReplacedDrugUnits chain or ParentVisitId
        public string LogicalVisitId { get; set; }

        public string DrugUnitId { get; set; }

        public string ReplacedDrugUnitId { get; set; }

        public string DrugUnitIdToRestore { get; set; }

        public bool? IsDrugUnitReplaced { get; set; }

        public bool? IsSelfSupportDrugUnit { get; set; }

        public int? RepeatKey { get; set; }

        public string TransactionType { get; set; }
    }
}
