using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.ViewsSql.Notifications;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Events;
using IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices.Models;
using Kernel.DDD.Domain.Events;

namespace IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices
{
    public class ReceivedNotificationDataService : StudyDataServiceBase<ReceivedNotificationViewModel>
    {
        public ReceivedNotificationDataService(
            IRTDbContext context,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IOptions<IrtAppConfigManager> appSettings)
            : base(context, extendedPropertiesValueProvider, appSettings)
        {
        }

        protected override void MapModelData(NotificationSqlView notification, Event e)
        {
            base.MapModelData(notification, e);

            var clientSubjectNotificationEvent = e as ClientNotificationReceived;
            if (clientSubjectNotificationEvent != null)
            {
                ModelData.Body = clientSubjectNotificationEvent.Body;
                ModelData.Title = clientSubjectNotificationEvent.Title;
            }
        }
    }
}
