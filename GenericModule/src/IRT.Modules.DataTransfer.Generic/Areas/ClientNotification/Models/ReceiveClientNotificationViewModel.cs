using System;

namespace IRT.Modules.DataTransfer.Generic.Areas.ClientNotification.Models
{
    public class ReceiveClientNotificationViewModel
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public string SiteId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SubjectVisitId { get; set; }

        public string AdditionalInfo { get; set; }
    }
}
