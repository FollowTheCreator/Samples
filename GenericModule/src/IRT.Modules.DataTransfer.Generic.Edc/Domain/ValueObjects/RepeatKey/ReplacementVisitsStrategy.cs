using System.ComponentModel.DataAnnotations;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey
{
    public enum ReplacementVisitsStrategy
    {
        [Display(Order = 0, Name = "Independent")]
        Independent = 0,

        [Display(Order = 1, Name = "Parent SubjectVisit")]
        ParentSubjectVisit = 1,

        [Display(Order = 2, Name = "Replaced Drug Unit Id")]
        ReplacedDrugUnitId = 2
    }
}
