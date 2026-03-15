using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kernel.EntityFramework.Interfaces;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.StudyEvent
{
    [Table("GenericStudyEventRepeatKeys")]
    public class GenericStudyEventRepeatKeySqlView : IDbModuleEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid RepeatKeyId { get; set; }

        public Guid NotificationDefinitionId { get; set; }

        public string SiteId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SubjectVisitId { get; set; }

        public int? RepeatKey { get; set; }

        public int? ScheduledRepeatKey { get; set; }

        public int? UnscheduledRepeatKey { get; set; }

        public int? ReplacementRepeatKey { get; set; }

        public int? ScreenFailRepeatKey { get; set; }

        public int? InformedConsentRepeatKey { get; set; }
    }
}
