using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kernel.EntityFramework.Interfaces;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.StudyEvent
{
    [Table("GenericStudyEventRepeatKeysLastUsed")]
    public class GenericStudyEventRepeatKeyLastUsedSqlView : IDbModuleEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid RepeatKeyLastUsedId { get; set; }

        public Guid NotificationDefinitionId { get; set; }

        public string SiteId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SubjectVisitId { get; set; }

        public int? RepeatKeyLastUsed { get; set; }

        public int? ScheduledRepeatKeyLastUsed { get; set; }

        public int? UnscheduledRepeatKeyLastUsed { get; set; }

        public int? ReplacementRepeatKeyLastUsed { get; set; }

        public int? ScreenFailRepeatKeyLastUsed { get; set; }

        public int? InformedConsentRepeatKeyLastUsed { get; set; }
    }
}
