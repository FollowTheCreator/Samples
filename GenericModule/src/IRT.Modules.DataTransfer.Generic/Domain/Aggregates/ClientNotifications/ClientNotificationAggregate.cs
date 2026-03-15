using System;
using Frameworks.Notifications;
using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Events;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications
{
    public class ClientNotificationAggregate : IRTStatelessAggregate
    {
        public void ClientNotificationSent(
            AcknowledgeClientNotification command,
            DateTime sentUtcDateTime)
        {
            AddDomainEvent(new ClientNotificationSent
            {
                ClientNotificationId = command.ClientNotificationId,
                SentUtcDateTime = sentUtcDateTime
            });
        }

        public void UpdateClientNotificationAdditionalInfo(AcknowledgeClientNotification command)
        {
            AddDomainEvent(new ClientNotificationAdditionalInfoUpdated
            {
                ClientNotificationId = command.ClientNotificationId,
                AdditionalInfo = command.ErrorCode
            });
        }

        public void ReceiveClientNotification(ReceiveClientNotification command)
        {
            AddDomainEvent(new ClientNotificationReceived
            {
               Title = command.Title,
               Body = command.Body,
               AdditionalInfo = command.AdditionalInfo,
               SiteId = command.SiteId,
               SubjectId = command.SubjectId,
               SubjectVisitId = command.SubjectVisitId,
               IsNotificationSent = command.IsNotificationSent
            });
        }

        public void UpdateClientNotification(
            UpdateClientNotification command,
            DateTime localDateTime,
            DateTime currentUTCTime)
        {
            AddDomainEvent(new ClientNotificationUpdated
            {
                NotificationId = command.NotificationId,
                Title = command.Title,
                Body = command.Body,
                FailedSendAttempts = command.FailedSendAttempts,
                IsNotificationSent = command.IsNotificationSent,
                TransactionLocalDateTime = localDateTime,
                TransactionUtcDateTime = currentUTCTime
            });
        }

        public void CreateClientNotification(
            CreateClientNotification command,
            string siteId,
            DateTime localDateTime,
            DateTime currentUTCTime)
        {
            AddDomainEvent(new ClientNotificationCreated
            {
                NotificationId = command.NotificationId,
                SiteId = siteId,
                SubjectId = command.SubjectId,
                SubjectVisitId = command.SubjectVisitId,
                VisitId = command.VisitId,
                Title = command.Title,
                Body = command.Body,
                IsNotificationSent = command.IsNotificationSent,
                NotificationDefinitionId = command.NotificationDefinitionId,
                TransactionLocalDateTime = localDateTime,
                TransactionUtcDateTime = currentUTCTime,
                DependentNotificationId = command.DependentNotificationId
            });
        }

        public void MarkNotificationsAsSent(MarkNotificationsAsSent command)
        {
            AddDomainEvent(new NotificationMarkedAsSent
            {
                NotificationIds = command.NotificationIds
            });
        }
    }
}
