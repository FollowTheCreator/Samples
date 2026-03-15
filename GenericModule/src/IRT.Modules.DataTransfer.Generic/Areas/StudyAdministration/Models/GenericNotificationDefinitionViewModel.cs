using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Frameworks.Notifications;
using IRT.Domain.ViewsSql;
using Kernel.AspNetMvc.Attributes;

namespace IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Models
{
    [DisplayNameGenerator(ResourceType = typeof(Resources.GenericNotificationDefinitionViewModel))]
    public class GenericNotificationDefinitionViewModel
    {
        public Guid NotificationDefinitionId { get; set; }

        [Required]
        [StringLength(DatabaseConstants.EntityNameMaxLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(DatabaseConstants.EntityNameMaxLength)]
        public string ApacheHopResourceEndpoint { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public string Title { get; set; }

        public string DataService { get; set; }

        public bool? UseDefaultTemplateProcessor { get; set; }

        public NotificationDefinition SelectedNotificationDefinition { get; set; }

        public IEnumerable<SelectListItem> DefaultNotificationDefinition { get; set; }

        public IEnumerable<SelectListItem> AvailableDataServices { get; set; }

        [Required]
        public string NotificationVendor { get; set; }

        [Required]
        public string NotificationType { get; set; }
    }
}
