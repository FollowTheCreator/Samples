using System.ComponentModel.DataAnnotations;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey
{
    public enum RepeatKeyCounterBasis
    {
        [Display(Order = 0, Name = "Drug Unit Id")]
        DrugUnitId = 0,

        [Display(Order = 1, Name = "SubjectVisit")]
        SubjectVisit = 1,

        [Display(Order = 2, Name = "Screen Fail Reason")]
        ScreenFailReason = 2
    }
}
