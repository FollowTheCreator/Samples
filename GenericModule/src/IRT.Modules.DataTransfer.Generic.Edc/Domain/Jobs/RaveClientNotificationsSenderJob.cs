using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain.Notifications;
using IRT.Domain.SchedulerJobs;
using IRT.Domain.ViewsSql.Study;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs.Resources;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Services.Implementations;
using Kernel.Globalization.Attributes;
using Kernel.Logging.Common;
using JobsResources = IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs.Resources;

namespace IRT.Modules.DataTransfer.Generic.Domain.Jobs
{
    [LocalizedCategory(JobsResources.RaveClientNotificationsSenderJob.ResourceNames.DataTransferGeneric, typeof(JobsResources.RaveClientNotificationsSenderJob))]
    [LocalizedDisplayName(JobsResources.RaveClientNotificationsSenderJob.ResourceNames.DisplayName, typeof(JobsResources.RaveClientNotificationsSenderJob))]
    [LocalizedDescription(JobsResources.RaveClientNotificationsSenderJob.ResourceNames.Description, typeof(JobsResources.RaveClientNotificationsSenderJob))]
    [Logs(Edc.Domain.Operations.GenericDataTransfer.ViewRaveJobsLogs)]
    public class RaveClientNotificationsSenderJob : BaseNotificationsEmailSenderJob
    {
        private readonly IExtendedPropertiesValueProvider extendedPropertiesValueProvider;
        private readonly RaveWebApiNotificationSenderService notificationSenderService;
        private readonly IQueryable<NotificationSqlView> notificationsQuery;

        public RaveClientNotificationsSenderJob(
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            RaveWebApiNotificationSenderService notificationSenderService,
            IQueryable<NotificationSqlView> notificationsQuery)
            : base(notificationsQuery, notificationSenderService)
        {
            this.extendedPropertiesValueProvider = extendedPropertiesValueProvider;
            this.notificationSenderService = notificationSenderService;
            this.notificationsQuery = notificationsQuery;
        }

        public override Task ExecuteInternal(IJobExecutionContext context)
        {
            var settings = extendedPropertiesValueProvider.GetValue<StudySqlView, GenericDataTransferStudySettings>(null);

            if (!settings.UseGenericDataTransfer)
            {
                Logger.Warn("The Generic Data Transfer is not enabled for this study.");
                return Task.CompletedTask;
            }

            Logger.Info(RaveClientSending.Sending_Started);

            notificationSenderService.MaxNumberOfRetries = settings.MaxNumberOfRetriesForClientNotifications;

            var allNotificationDefinitions = NotificationDefinitionRegistry.GetNotificationDefinitions(null);
            var notificationDefinitions = new List<Guid>();

            notificationDefinitions.AddRange(allNotificationDefinitions
                .Where(n => n.PendingNotificationSenderTypes.Contains(notificationSenderService.GetType()))
                .Select(n => n.Id));

            var notificationsToFilter = notificationsQuery
                .Where(x => notificationDefinitions.Contains(x.NotificationDefinitionId))
                .Include(x => x.NotificationLocalizedContentEntries)
                .Include(x => x.NotificationDefinition);

            var notifications = notificationSenderService
                .FilterPendingNotifications(notificationsToFilter)
                .OrderBy(x => x.GeneratedUtcDateTime)
                .Take(100)
                .ToList();

            Logger.Info(RaveClientSending.Sending_Attempting, notifications.Count);

            notifications.ForEach(x => notificationSenderService.SendNotification(y => NotificationDefinitionRegistry.GetDefinition(y), x));

            Logger.Info(RaveClientSending.Sending_Finished);

            return Task.CompletedTask;
        }
    }
}