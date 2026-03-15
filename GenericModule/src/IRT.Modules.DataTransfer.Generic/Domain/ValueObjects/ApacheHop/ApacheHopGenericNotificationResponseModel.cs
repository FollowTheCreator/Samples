using System.Collections.Generic;

namespace IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.ApacheHop
{
    public class ApacheHopGenericNotificationResponseModel
    {
        public List<ApacheHopGenericNotificationExecutionResult> ExecutionResult { get; set; }
    }

    public class ApacheHopGenericNotificationExecutionResult
    {
        public ApacheHopExecutionStatus Status { get; set; }

        public string ClientPayload { get; set; }

        public int? NoErrors { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorFields { get; set; }

        public string ErrorDesc { get; set; }

        public string RequestType { get; set; }

        public string Title { get; set; }
    }
}
