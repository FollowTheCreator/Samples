using System;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey
{
    public class MaxStudyEventRepeatKeyModel
    {
        public int? MaxRepeatKey { get; set; }

        public int? MaxScheduledRepeatKey { get; set; }

        public int? MaxUnscheduledRepeatKey { get; set; }

        public int? MaxReplacementRepeatKey { get; set; }

        public int? MaxScreenFailRepeatKey { get; set; }

        public int? MaxInformedConsentRepeatKey { get; set; }

        public Guid? RepeatKeyLastUsedId { get; set; }
    }
}
