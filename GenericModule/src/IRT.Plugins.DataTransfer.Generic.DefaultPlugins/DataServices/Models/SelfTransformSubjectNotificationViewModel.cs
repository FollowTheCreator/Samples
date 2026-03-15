using System;
using IRT.Domain.Notifications.Models.Subject;

namespace IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DataServices.Models
{
    public class SelfTransformSubjectNotificationViewModel : SubjectViewModel
    {
        public Guid NotificationId { get; set; }

        public string CreationDateTime { get; set; }

        public string JsonModel { get; set; }
    }
}
