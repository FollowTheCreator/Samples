using System;
using Kernel.DDD.Domain.Events;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Events
{
    public class DataTransferErrorNotificationCreated : Event
    {
        public Guid NotificationId { get; set; }

        public string Title { get; set; }

        public string SiteId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SubjectVisitId { get; set; }

        public string ErrorCode { get; set; }

        public string Url { get; set; }

        public string PostedMessage { get; set; }

        public string Response { get; set; }

        public string FileOID { get; set; }
    }
}
