using System;
using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Events;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands.Veeva
{
    public class VeevaNotificationAggregate : IRTStatelessAggregate
    {
        public void CreateVeevaClientErrorNotification(
            VeevaAcknowledgeClientNotification command,
            string postedMessage,
            string notificationTitle)
        {
            var notification = Db.Notifications.Find(command.ClientNotificationId);

            AddDomainEvent(new DataTransferErrorNotificationCreated
            {
                Response = command.ResponseMessage,
                ErrorCode = command.ErrorCode,
                Url = command.Url,
                PostedMessage = postedMessage,
                Title = notificationTitle,
                SiteId = notification.SiteId,
                SubjectId = notification.SubjectId,
                SubjectVisitId = notification.SubjectVisitId,
                FileOID = command.FileOid,
            });
        }
    }
}
