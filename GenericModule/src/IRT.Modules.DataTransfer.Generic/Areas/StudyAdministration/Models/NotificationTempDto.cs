using System;

namespace IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Models
{
    public class NotificationTempDto
    {
        public Guid Id { get; set; }

        public Guid? NotificationDefinitionId { get; set; }

        public string NotificationDefinitionName { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Status { get; set; }

        public DateTime? SentUtcDateTime { get; set; }

        public DateTime GeneratedUtcDateTime { get; set; }

        public bool IsNotificationSent { get; set; }

        public int FailedSendAttempts { get; set; }

        public string AdditionalInfo { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SubjectVisitId { get; set; }
    }
}
