using IRT.Domain.Notifications.Models;

namespace IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices.Models
{
    public class ReceivedNotificationViewModel : StudyViewModel
    {
        public string Body { get; set; }

        public string Title { get; set; }
    }
}
