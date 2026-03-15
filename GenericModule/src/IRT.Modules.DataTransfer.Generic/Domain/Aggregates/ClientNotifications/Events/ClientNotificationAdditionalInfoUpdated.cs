using System;
using Kernel.DDD.Domain.Events;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Events
{
    public class ClientNotificationAdditionalInfoUpdated : Event
    {
        public Guid ClientNotificationId { get; set; }

        public string AdditionalInfo { get; set; }
    }
}
