using System;
using System.Collections.Generic;
using IRT.Domain;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Events
{
    public class NotificationMarkedAsSent : IRTEvent
    {
        public IList<Guid> NotificationIds { get; set; }
    }
}
