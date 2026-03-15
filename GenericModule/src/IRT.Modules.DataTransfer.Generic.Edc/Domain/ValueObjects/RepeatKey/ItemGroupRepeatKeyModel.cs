using System;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey
{
    public class ItemGroupRepeatKeyModel
    {
        public int? RepeatKey { get; set; }

        public Guid? RepeatKeyLastUsedId { get; set; }

        public Guid? LogicalSubjectVisitId { get; set; }

        public string LogicalVisitId { get; set; }
    }
}
