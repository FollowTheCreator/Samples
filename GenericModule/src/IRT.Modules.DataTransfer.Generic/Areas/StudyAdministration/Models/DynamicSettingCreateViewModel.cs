using System.ComponentModel.DataAnnotations;
using Kernel.AspNetMvc.Attributes;

namespace IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Models
{
    [DisplayNameGenerator(ResourceType = typeof(Resources.DynamicSettingCreateViewModel))]
    public class DynamicSettingCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string DefaultValue { get; set; }

        [Required]
        public string EntityTypeName { get; set; }

        [Required]
        public string ExtendedPropertyName { get; set; }
    }
}
