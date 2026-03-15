using System;
using System.ComponentModel.DataAnnotations;

namespace IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Models
{
    public class ClientNotificationUpdateViewModel : ElectronicSignatureViewModel
    {
        public Guid NotificationId { get; set; }

        [Display(Name = "Notification Definition")]
        public string NotificationDefinitionName { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Body")]
        public string Body { get; set; }

        [Display(Name = "Is notification sent?")]
        public bool IsNotificationSent { get; set; }

        [Display(Name = "Number of failed attempts to send")]
        public int FailedSubmitAttempts { get; set; }

        [Display(Name = "Dependent Notification")]
        public string DependentNotification { get; set; }
    }
}
