using System;
using Kernel.DDD.Domain.Commands;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands
{
    public class ReceiveClientNotification : Command
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public string SiteId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SubjectVisitId { get; set; }

        public bool IsNotificationSent { get; set; }

        public string AdditionalInfo { get; set; }
    }
}
