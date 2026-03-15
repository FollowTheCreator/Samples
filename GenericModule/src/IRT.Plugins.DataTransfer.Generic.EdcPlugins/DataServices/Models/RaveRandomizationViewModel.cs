using IRT.Domain.ViewsSql.Subject;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Models;
using System.Collections.Generic;
using System.Linq;

namespace IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.Models
{
    public class RaveRandomizationViewModel : GenericBaseSubjectViewModel
    {
        public IList<SubjectVisitCapturedDataSqlView> SubjectVisitCapturedDataQuery;

        public string RandomizationNumberId { get; set; }

        public string RandomizationDate { get; set; }
    }
}
