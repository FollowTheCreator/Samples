using System.Collections.Generic;
using IRT.Domain.Notifications.Models;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.Errors;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Models
{
    public class ClientDataTransferErrorSummaryNotificationViewModel : StudyViewModel
    {
        public IEnumerable<ErrorListData> Notifications { get; set; }

        public string TitleType { get; set; }
    }
}
