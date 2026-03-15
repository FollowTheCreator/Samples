using System.ComponentModel.DataAnnotations;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey
{
    public enum UnscheduledVisitsStrategy
    {
        [Display(Order = 0, Name = "Independent")]
        Independent = 0,

        [Display(Order = 1, Name = "Parent SubjectVisit")]
        ParentSubjectVisit = 1
    }
}
