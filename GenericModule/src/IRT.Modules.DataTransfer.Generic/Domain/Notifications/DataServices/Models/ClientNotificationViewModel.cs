using System;
using IRT.Domain.Notifications.Models;

namespace IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices.Models
{
    public class ClientNotificationViewModel : StudyViewModel
    {
        public Guid GenericNotificationId { get; set; }

        public string Body { get; set; }

        public string Title { get; set; }
    }
}
