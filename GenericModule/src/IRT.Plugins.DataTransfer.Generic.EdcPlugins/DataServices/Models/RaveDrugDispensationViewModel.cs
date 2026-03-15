using System.Collections.Generic;
using IRT.Domain.ViewsSql.Subject;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Models;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;

namespace IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.Models
{
    public class RaveDrugDispensationViewModel : GenericBaseSubjectViewModel
    {
        public SubjectVisitSqlView SubjectVisitSqlView { get; set; }

        public IList<SerializableDrugUnit> SerializableDrugUnitList { get; set; }
    }
}
