using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;
using Kernel.EntityFramework.Interfaces;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup
{
    [Table("GenericItemGroupRepeatKeysLastUsed")]
    public class GenericItemGroupRepeatKeyLastUsedSqlView : IDbModuleEntity
    {
        public GenericItemGroupRepeatKeyLastUsedSqlView()
        {
        }

        public GenericItemGroupRepeatKeyLastUsedSqlView(
            int repeatKey,
            ItemGroupRepeatKeyFilterModel repeatKeyFilter)
        {
            RepeatKeyLastUsedId = Guid.NewGuid();

            NotificationDefinitionId = repeatKeyFilter.NotificationDefinitionId;

            SubjectId = repeatKeyFilter.SubjectId;

            SubjectVisitId = repeatKeyFilter.SubjectVisitId;

            LogicalSubjectVisitId = repeatKeyFilter.SubjectVisitId;

            VisitId = repeatKeyFilter.VisitId;

            LogicalVisitId = repeatKeyFilter.VisitId;

            SiteId = repeatKeyFilter.SiteId;

            RepeatKeyLastUsed = repeatKey;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid RepeatKeyLastUsedId { get; set; }

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

        public int? RepeatKeyLastUsed { get; set; }
    }
}
