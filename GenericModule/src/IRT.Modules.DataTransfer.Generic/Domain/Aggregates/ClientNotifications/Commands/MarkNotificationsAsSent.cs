using System;
using System.Collections.Generic;
using Kernel.DDD.Domain.Commands;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands
{
    public class MarkNotificationsAsSent : Command
    {
        public IList<Guid> NotificationIds { get; set; }
    }
}
