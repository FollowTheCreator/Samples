using System;
using Kernel.DDD.Domain.Commands;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands
{
    public class AcknowledgeClientNotification : Command
    {
        public Guid ClientNotificationId { get; set; }

        public string ErrorCode { get; set; }

        public string ResponseMessage { get; set; }

        public bool IsSuccess { get; set; }

        public string Url { get; set; }
    }
}
