using IRT.Domain.ViewsSql.Notifications;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands.Veeva;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Events;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Events.Veeva;
using NotificationDefinitions = IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.NotificationDefinitions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.SqlViews
{
    public class VeevaClientNotificationSqlViewHandler : NotificationSqlViewHandlerBase
    {
        public void Handle(DataTransferErrorNotificationCreated e)
        {
            if (e.CommandMetadata.TypeName == typeof(VeevaAcknowledgeClientNotification).FullName)
            {
                AddNotification(
                NotificationDefinitions.VeevaErrorNotification,
                e,
                setAdditionalPropertiesAction: x =>
                {
                    x.SiteId = e.SiteId;
                    x.SubjectId = e.SubjectId;
                    x.SubjectVisitId = e.SubjectVisitId;
                });
            }
        }

        public void Handle(VeevaDataTransferErrorSummaryNotificationCreated e)
        {
            AddNotification(NotificationDefinitions.VeevaErrorsListNotification, e);
        }
    }
}
