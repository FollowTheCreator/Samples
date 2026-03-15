using System;
using Frameworks.Notifications;
using IRT.Domain;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Events
{
    public class ClientNotificationUpdated : IRTEvent
    {
        public Guid NotificationId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public int? FailedSendAttempts { get; set; }

        public bool IsNotificationSent { get; set; }
    }
}
