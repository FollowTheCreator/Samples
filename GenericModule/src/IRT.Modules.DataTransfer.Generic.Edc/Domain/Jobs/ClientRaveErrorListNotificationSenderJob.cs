using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NLog;
using Quartz;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain.Notifications;
using IRT.Domain.ViewsSql.Study;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDefinition;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs.Base;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs.Resources;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Resources.RaveResources;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Services.Implementations;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Helpers.Infrastucture;
using Kernel.DDD.Dispatching;
using Kernel.Globalization.Attributes;
using Kernel.Logging.Common;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs
{
    [LocalizedCategory(Resources.ClientRaveErrorListNotificationGeneratorJob.ResourceNames.DataTransferGeneric, typeof(Resources.ClientRaveErrorListNotificationGeneratorJob))]
    [LocalizedDisplayName(Resources.ClientRaveErrorListNotificationGeneratorJob.ResourceNames.DisplayName, typeof(Resources.ClientRaveErrorListNotificationGeneratorJob))]
    [LocalizedDescription(Resources.ClientRaveErrorListNotificationGeneratorJob.ResourceNames.Description, typeof(Resources.ClientRaveErrorListNotificationGeneratorJob))]
    [Logs(Edc.Domain.Operations.GenericDataTransfer.ViewRaveJobsLogs)]
    public class ClientRaveErrorListNotificationSenderJob : BaseFailedNotificationsSenderJob
    {
        private readonly IExtendedPropertiesValueProvider extendedPropertiesValueProvider;
        private readonly RaveWebApiNotificationSenderService notificationSenderService;
        private readonly IQueryable<NotificationSqlView> notificationsQuery;
        private readonly IGenericNotificationDefinitionService genericNotificationDefinitionService;
        private readonly ICommandBus commandBus;

        public ClientRaveErrorListNotificationSenderJob(
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            RaveWebApiNotificationSenderService notificationSenderService,
            IQueryable<NotificationSqlView> notificationsQuery,
            IGenericNotificationDefinitionService genericNotificationDefinitionService,
            ICommandBus commandBus)
            : base(commandBus, extendedPropertiesValueProvider, notificationsQuery, notificationSenderService)
        {
            this.extendedPropertiesValueProvider = extendedPropertiesValueProvider;
            this.notificationSenderService = notificationSenderService;
            this.notificationsQuery = notificationsQuery;
            this.genericNotificationDefinitionService = genericNotificationDefinitionService;
            this.commandBus = commandBus;
        }

        protected override bool CanSendNotifications(IJobExecutionContext context)
        {
            if (!extendedPropertiesValueProvider.GetValue<StudySqlView, GenericDataTransferStudySettings>(null).UseGenericDataTransfer)
            {
                Logger.Warn("The Generic data transfer is not enabled for this study");
                return false;
            }

            var studySettings = extendedPropertiesValueProvider.GetValue<StudySqlView, GenericEdcDataTransferStudySettings>(null);
            if (!studySettings.EnableRaveErrorNotification)
            {
                Logger.Warn("The Rave Error Notifications are not enabled for this study");
                return false;
            }

            return true;
        }


        protected override IQueryable<NotificationSqlView> GetFailedNotificationsQuery(IJobExecutionContext context)
        {
            var errorCodesFromResources = RaveErrorMessagesInfoResources.ResourceManager
                    .GetResourceManagerKeys(
                        CultureInfo.InvariantCulture,
                        true,
                        true);
            var errorCodes = errorCodesFromResources;

            // with backout info possibly saved in the additional info column, we want to search through all the data in the column to see if the error code exists
            var notificationsWithAdditionalInfo = notificationSenderService
                .FilterPendingNotifications(notificationsQuery)
                .Where(x => genericNotificationDefinitionService.GetIdsFor(NotificationVendor.Rave).Contains(x.NotificationDefinitionId))
                .Include(x => x.NotificationLocalizedContentEntries)
                .Include(x => x.NotificationDefinition)
                .Where(x => x.AdditionalInfo != null);

            return notificationsWithAdditionalInfo
                .Where(x => errorCodes.Any(y => x.AdditionalInfo.Contains(y)))
                .OrderBy(x => x.GeneratedUtcDateTime);
        }

        protected override void SendFailedNotifications(IJobExecutionContext context)
        {
            this.Logger.Info(RaveClientSending.Sending_Started);

            var notifications = this.NotificationSenderService
                .FilterPendingNotifications(this.NotificationsQuery)
                .Include(x => x.NotificationDefinition)
                .Include(x => x.NotificationLocalizedContentEntries)
                .OrderBy(x => x.GeneratedUtcDateTime)
                .ToArray();
            this.Logger.Info(RaveClientSending.Sending_Attempting, notifications.Length);

            notifications.ForEach(x => NotificationSenderService
                .SendNotification(NotificationDefinitionRegistry.GetDefinition, x));

            this.Logger.Info(RaveClientSending.Sending_Finished);
        }


        // need a better way to do this
        protected override void SendFailedNotificationsSummary(IJobExecutionContext context)
        {
            base.SendFailedNotificationsSummary(context);
        }

        protected override IEnumerable<NotificationSqlView> FilterFailedNotifications(
            IJobExecutionContext context, IEnumerable<NotificationSqlView> notifications)
            => notifications;
    }
}