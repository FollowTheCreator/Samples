using IRT.Domain.Notifications.Models;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.Errors;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Models
{
    public class ClientDataTransferErrorNotificationViewModel : StudyViewModel
    {
        public ErrorData ErrorData { get; set; }
    }
}
