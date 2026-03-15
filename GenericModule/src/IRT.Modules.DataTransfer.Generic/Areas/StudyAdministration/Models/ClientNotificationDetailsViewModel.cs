using System;
using Frameworks.Notifications;
using IRT.Domain.ValueObjects.Notifications;

namespace IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Models
{
    public class ClientNotificationDetailsViewModel
    {
        public Notification Notification { get; set; }

        public NotificationLocalizedContent NotificationContent { get; set; }

        public NotificationDefinition NotificationDefinition { get; set; }

        public string DependentNotification { get; set; }
    }
}
