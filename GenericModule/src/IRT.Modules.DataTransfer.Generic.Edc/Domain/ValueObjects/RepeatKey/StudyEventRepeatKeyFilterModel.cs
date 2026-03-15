using System;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey
{
    public class StudyEventRepeatKeyFilterModel
    {
        public Guid NotificationDefinitionId { get; set; }

        public RepeatKeyBasis RepeatKeyBasis { get; set; }

        public bool ReuseRepeatKey { get; set; }

        public int RepeatKeyInitialValue { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SubjectVisitId { get; set; }

        public string VisitId { get; set; }

        public bool IsUnscheduled { get; set; }

        public string SiteId { get; set; }
    }
}
