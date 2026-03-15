using IRT.Domain.ViewsSql.Subject.SubjectVisitDrug;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey
{
    public class SerializableDrugUnit
    {
        public SubjectVisitLabeledDrugSqlView SubjectVisitLabeledDrug { get; set; }

        public SubjectVisitLabeledDrugSqlView SubjectVisitLabeledDrugToRestore { get; set; }

        public SerializableRepeatKey AssignedItemGroupRepeatKey { get; set; }

        public SerializableRepeatKey ReplacedItemGroupRepeatKey { get; set; }
    }

    public class SerializableRepeatKey
    {
        public string RepeatKey { get; set; }

        public string TransactionType { get; set; }
    }
}
