using System;

namespace IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.Dto
{
    public class SubjectInfoOnBackout
    {
        public Guid? SubjectId { get; set; }

        public string SubjectNumber { get; set; }

        public string ScreeningNumber { get; set; }

        public Guid? SubjectVisitId { get; set; }

        public string ErrorCode { get; set; }
    }
}
