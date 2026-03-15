using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.ViewsSql.Notifications;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Events;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Models;
using Kernel.DDD.Domain.Events;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Errors
{
    public class ClientDataTransferErrorSummaryNotificationDataService : StudyDataServiceBase<ClientDataTransferErrorSummaryNotificationViewModel>
    {
        public ClientDataTransferErrorSummaryNotificationDataService(
            IRTDbContext context,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IOptions<IrtAppConfigManager> appSettings)
            : base(context, extendedPropertiesValueProvider, appSettings)
        {
        }

        protected override void MapModelData(NotificationSqlView notification, Event e)
        {
            base.MapModelData(notification, e);

            var eventName = e.GetType().Name;

            if (eventName.Contains("Veeva"))
            {
                ModelData.TitleType = "Veeva";
            }
            else if(eventName.Contains("Rave"))
            {
                ModelData.TitleType = "Rave";
            }

            var errorListNotificationCreated = e as DataTransferErrorSummaryNotificationCreated;

            if (errorListNotificationCreated != null)
            {
                ModelData.Notifications = errorListNotificationCreated.FailedNotifcations;
            }
        }
    }
}
