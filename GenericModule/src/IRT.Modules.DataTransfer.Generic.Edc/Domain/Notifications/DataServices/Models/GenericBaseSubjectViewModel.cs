using System;
using IRT.Domain.Notifications.Models.Subject;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Models
{
    public class GenericBaseSubjectViewModel : SubjectViewModel
    {
        public Guid NotificationId { get; set; }

        public string CreationDateTime { get; set; }

        public string JsonSubject { get; set; }

        public string JsonSite { get; set; }

        public string JsonDemographicCountrySettings { get; set; }

        public string JsonNotification { get; set; }

        public string JsonStudy { get; set; }

        public string JsonStudySettings { get; set; }

        public string JsonStudyLimits { get; set; }

        public string JsonSelfSupportModificationRequest { get; set; }

        public string JsonNotificationGenerationSettingEntity { get; set; }

        public string FormRepeatKey { get; set; }

        public string StudyEventRepeatKey { get; set; }

        public SerializableRepeatKey ItemGroupRepeatKey { get; set; }

        public bool IsBackout { get; set; }

        public string VisitEdcMapping { get; set; }
    }
}
