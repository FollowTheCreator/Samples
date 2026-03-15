using System;
using Kernel.DDD.Domain.Commands;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands
{
    public class CreateClientNotification : Command
    {
        public Guid NotificationId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public Guid SubjectId { get; set; }

        public Guid? SubjectVisitId { get; set; }

        public string VisitId { get; set; }

        public Guid NotificationDefinitionId { get; set; }

        public bool IsNotificationSent { get; set; }

        public Guid? DependentNotificationId { get; set; }
    }
}
