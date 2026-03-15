using System;
using System.ComponentModel.DataAnnotations;

namespace IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Models
{
    public class ClientNotificationCreateViewModel : ElectronicSignatureViewModel
    {
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        [Display(Name = "Notification Definition")]
        public Guid? NotificationDefinitionId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public Guid? SubjectId { get; set; }

        [Display(Name = "Subject Visit")]
        public Guid? SubjectVisitId { get; set; }

        [Display(Name = "Dependent Notification")]
        public Guid? DependentNotificationId { get; set; }
    }
}
