using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands.Rave
{
    public class RaveAknowledgeClientNotification : AcknowledgeClientNotification
    {
        public string FileOid { get; set; }
    }
}
