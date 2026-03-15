using System;

namespace IRT.Modules.DataTransfer.Generic.Areas.ClientNotification.Models
{
    public class AcknowledgeViewModel
    {
        public Guid ClientNotificationId { get; set; }

        public string ErrorCode { get; set; }

        public string ResponseMessage { get; set; }

        public bool IsSuccess { get; set; }

        public string Url { get; set; }

        public string FileOid { get; set; }
    }
}
