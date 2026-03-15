using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.StudyEvent;
using IRT.Modules.DataTransfer.Generic.Edc.Helpers.RepeatKey;
using IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.Models;
using Kernel.DDD.Domain.Events;

namespace IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices
{
    [Description("RaveDemographicsNotificationDataService")]
    public class RaveDemographicsNotificationDataService : BaseGenericNotificationDataService<RaveDemographicsViewModel>
    {
        public RaveDemographicsNotificationDataService(
            IRTDbContext context,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IOptions<IrtAppConfigManager> appSettings,
            IQueryable<GenericStudyEventRepeatKeySqlView> genericStudyEventRepeatKeysQuery,
            IQueryable<GenericItemGroupRepeatKeySqlView> genericItemGroupRepeatKeysQuery,
            StudyEventRepeatKeyHelper studyEventRepeatKeyHelper,
            ItemGroupRepeatKeyHelper itemGroupRepeatKeyHelper)
            : base(
                context,
                extendedPropertiesValueProvider,
                appSettings,
                genericStudyEventRepeatKeysQuery,
                genericItemGroupRepeatKeysQuery,
                studyEventRepeatKeyHelper,
                itemGroupRepeatKeyHelper)
        {
        }

        public override void MapModelDataInternal(NotificationSqlView notification, Event e)
        {
            SetRepeatKeys(notification);
        }
    }
}
