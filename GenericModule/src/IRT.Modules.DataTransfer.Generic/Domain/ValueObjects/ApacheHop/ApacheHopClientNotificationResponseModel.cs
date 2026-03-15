using System.Collections.Generic;

namespace IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.ApacheHop
{
    public class ApacheHopClientNotificationResponseModel
    {
        public List<ApacheHopClientNotificationExecutionResult> ExecutionResult { get; set; }
    }

    public class ApacheHopClientNotificationExecutionResult
    {
        public bool IsSuccess { get; set; }

        public string ErrorCode { get; set; }

        public string ResponseMessage { get; set; }

        public string Url { get; set; }

        public string FileOid { get; set; }
    }
}
