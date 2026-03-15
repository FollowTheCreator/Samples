using System.ComponentModel.DataAnnotations;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey
{
    public enum RepeatKeyBasis
    {
        [Display(Order = 0, Name = "Study")]
        Study = 0,

        [Display(Order = 1, Name = "Site")]
        Site = 1,

        [Display(Order = 2, Name = "Subject")]
        Subject = 2,

        [Display(Order = 3, Name = "SubjectVisit")]
        SubjectVisit = 3
    }
}
