using System;
using Kernel.DDD.Domain.Events;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Events
{
    public class ClientNotificationSent : Event
    {
        public Guid ClientNotificationId { get; set; }

        public DateTime SentUtcDateTime { get; set; }
    }
}
