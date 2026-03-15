using IRT.Domain.Notifications.Models;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Models
{
    public class ClientRaveErrorNotificationViewModel : StudyViewModel
    {
        public string ErrorCode { get; set; }

        public string Url { get; set; }

        public string PostedMessage { get; set; }

        public string Response { get; set; }

        public string ErrorTitle { get; set; }

        public string FileOid { get; set; }
    }
}
