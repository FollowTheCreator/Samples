using System;
using Frameworks.Notifications;
using Kernel.DDD.Domain.Commands;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands
{
    public class CreateDataTransferErrorNotification : Command
    {
        public string Body { get; set; }

        public Guid NotificationDefinitionId { get; set; }

        public NotificationDefinition NotificationDefinition { get; set; }

        public bool IsNotificationSent { get; set; }
    }
}
