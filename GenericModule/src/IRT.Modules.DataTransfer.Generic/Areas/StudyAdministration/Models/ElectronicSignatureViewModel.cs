using System.ComponentModel.DataAnnotations;

namespace IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Models
{
    //TODO: change this to IRT version that includes SSO
    public class ElectronicSignatureViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string SignatureUsername { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string SignaturePassword { get; set; }
    }
}
