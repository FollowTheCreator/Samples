using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.ViewsSql.Notifications;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Events;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Models;
using IRT.Modules.DataTransfer.Generic.Helpers.Extensions;
using Kernel.DDD.Domain.Events;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Errors
{
    public class ClientDataTransferErrorNotificationDataService : StudyDataServiceBase<ClientRaveErrorNotificationViewModel>
    {
        public ClientDataTransferErrorNotificationDataService(
            IRTDbContext context,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IOptions<IrtAppConfigManager> appSettings)
            : base(context, extendedPropertiesValueProvider, appSettings)
        {
        }

        protected override void MapModelData(NotificationSqlView notification, Event e)
        {
            base.MapModelData(notification, e);

            var clientErrorNotificationCreated = e as DataTransferErrorNotificationCreated;

            if (clientErrorNotificationCreated == null)
            {
                return;
            }

            ModelData.ErrorCode = clientErrorNotificationCreated.ErrorCode;
            ModelData.Url = clientErrorNotificationCreated.Url;
            ModelData.PostedMessage = clientErrorNotificationCreated.PostedMessage.FormatToXml();
            ModelData.Response = clientErrorNotificationCreated.Response.FormatToXml();
            ModelData.FileOid = clientErrorNotificationCreated.FileOID;
            ModelData.ErrorTitle = clientErrorNotificationCreated.Title + (!string.IsNullOrEmpty(clientErrorNotificationCreated.FileOID)
                ? (": " + clientErrorNotificationCreated.FileOID)
                : string.Empty);
        }
    }
}
