using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands.Veeva
{
    public class VeevaAcknowledgeClientNotification : AcknowledgeClientNotification
    {
        public string FileOid { get; set; }
    }
}
