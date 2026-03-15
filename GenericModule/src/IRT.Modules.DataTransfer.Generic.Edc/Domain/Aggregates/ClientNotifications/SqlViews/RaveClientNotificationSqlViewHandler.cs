using IRT.Domain.ViewsSql.Notifications;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Events;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands.Rave;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Events;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Events.Rave;
using NotificationDefinitions = IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.NotificationDefinitions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.SqlViews
{
    public class RaveClientNotificationSqlViewHandler : NotificationSqlViewHandlerBase
    {
        public void Handle(DataTransferErrorNotificationCreated e)
        {
            if (e.CommandMetadata.TypeName == typeof(RaveAknowledgeClientNotification).FullName)
            {
                AddNotification(
                    NotificationDefinitions.RaveErrorNotification,
                    e,
                    setAdditionalPropertiesAction: x =>
                    {
                        x.SiteId = e.SiteId;
                        x.SubjectId = e.SubjectId;
                        x.SubjectVisitId = e.SubjectVisitId;
                    });
            }
        }

        public void Handle(RaveDataTransferErrorSummaryNotificationCreated e)
        {
            AddNotification(NotificationDefinitions.RaveErrorsListNotification, e);
        }

        public void Handle(ClientNotificationReceived e)
        {
            AddNotification(
                NotificationDefinitions.RaveClientEdcReceivedNotification,
                e,
                setAdditionalPropertiesAction: x =>
                {
                    x.SiteId = e.SiteId;
                    x.SubjectId = e.SubjectId;
                    x.SubjectVisitId = e.SubjectVisitId;
                    x.AdditionalInfo = e.AdditionalInfo;
                });
        }
    }
}
