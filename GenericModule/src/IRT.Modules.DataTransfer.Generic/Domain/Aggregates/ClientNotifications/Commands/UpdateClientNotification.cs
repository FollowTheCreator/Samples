using System;
using Kernel.DDD.Domain.Commands;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands
{
    public class UpdateClientNotification : Command
    {
        public Guid NotificationId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public int? FailedSendAttempts { get; set; }

        public string SiteId { get; set; }

        public bool IsNotificationSent { get; set; }
    }
}
