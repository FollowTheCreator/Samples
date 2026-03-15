using System;
using Kernel.AspNetMvc.Attributes;

namespace IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Models
{
    [DisplayNameGenerator(ResourceType = typeof(Resources.GenericNotificationViewModel))]
    public class GenericNotificationViewModel
    {
        public Guid Id { get; set; }

        public string NotificationDefinitionName { get; set; }

        public string Title { get; set; }

        public DateTime? GeneratedUtcDateTime { get; set; }

        public string Status { get; set; }

        public DateTime? SentUtcDateTime { get; set; }

        public int? FailedSendAttempts { get; set; }

        public string AdditionalInfo { get; set; }
    }
}
