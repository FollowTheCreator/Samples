using System;
using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.ViewsSql.Notifications;
using IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices.Interfaces;
using IRT.Modules.DataTransfer.Generic.Helpers.Extensions;
using IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DataServices.Models;
using Kernel.DDD.Domain.Events;

namespace IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DataServices
{
    [Description("SelfTransformSubjectNotificationDataService")]
    public class SelfTransformSubjectNotificationDataService : SubjectDataServiceBase<SelfTransformSubjectNotificationViewModel>, IGenericNotificationDataService
    {
        public SelfTransformSubjectNotificationDataService(
             IRTDbContext context,
             IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
             IOptions<IrtAppConfigManager> appSettings)
             : base(context, extendedPropertiesValueProvider, appSettings)
        {
        }

        public void MapModelDataInternal(NotificationSqlView notification, Event e)
        {
            throw new NotImplementedException();
        }

        protected override void MapModelData(NotificationSqlView notification, Event e)
        {
            base.MapModelData(notification, e);

            ModelData.NotificationId = notification.Id;
            ModelData.CreationDateTime = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");

            string jsonModel = JsonSerializer.Serialize(ModelData);
            ModelData.JsonModel = jsonModel.JsonPrettify();
        }
    }
}
