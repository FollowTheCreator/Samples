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
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDependency;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Services.Implementations;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;
using Kernel.Globalization.Attributes;
using Kernel.Logging.Common;
using JobsResources = IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs.Resources;

namespace IRT.Modules.DataTransfer.Generic.Domain.Jobs
{
    [LocalizedCategory(JobsResources.VeevaClientNotificationsSenderJob.ResourceNames.DataTransferGeneric, typeof(JobsResources.VeevaClientNotificationsSenderJob))]
    [LocalizedDisplayName(JobsResources.VeevaClientNotificationsSenderJob.ResourceNames.DisplayName, typeof(JobsResources.VeevaClientNotificationsSenderJob))]
    [LocalizedDescription(JobsResources.VeevaClientNotificationsSenderJob.ResourceNames.Description, typeof(JobsResources.VeevaClientNotificationsSenderJob))]
    [Logs(Edc.Domain.Operations.GenericDataTransfer.ViewVeevaJobsLogs)]
    public class VeevaClientNotificationsSenderJob : BaseNotificationsEmailSenderJob
    {
        private readonly IExtendedPropertiesValueProvider extendedPropertiesValueProvider;
        private readonly VeevaWebApiNotificationSenderService notificationSenderService;
        private readonly IQueryable<NotificationSqlView> notificationsQuery;
        private readonly IGenericNotificationDependencyService genericNotificationDependencyService;

        public VeevaClientNotificationsSenderJob(
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            VeevaWebApiNotificationSenderService notificationSenderService,
            IQueryable<NotificationSqlView> notificationsQuery,
            IGenericNotificationDependencyService genericNotificationDependencyService)
            : base(notificationsQuery, notificationSenderService)
        {
            this.extendedPropertiesValueProvider = extendedPropertiesValueProvider;
            this.notificationSenderService = notificationSenderService;
            this.notificationsQuery = notificationsQuery;
            this.genericNotificationDependencyService = genericNotificationDependencyService;
        }

        public override Task ExecuteInternal(IJobExecutionContext context)
        {
            var settings = extendedPropertiesValueProvider.GetValue<StudySqlView, GenericDataTransferStudySettings>(null);

            if (!settings.UseGenericDataTransfer)
            {
                Logger.Warn("The Generic Data Transfer is not enabled for this study.");
                return Task.CompletedTask;
            }

            Logger.Info("Started sending Veeva Client notifications.");

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

            Logger.Info("Found {0} Veeva Client notifications to send (bulk limit is 100).", notifications.Count);

            foreach (var notification in notifications)
            {
                var genericNotificationDependency = genericNotificationDependencyService.GetNotificationDependencyFor(notification.Id);
                if (genericNotificationDependency != null)
                {
                    if (genericNotificationDependency.DependsOnId == null)
                    {
                        notificationSenderService.SendNotification(y => NotificationDefinitionRegistry.GetDefinition(y), notification);
                    }
                    else
                    {
                        var parentNotification = notificationsQuery.First(n => n.Id == genericNotificationDependency.DependsOnId);
                        if (parentNotification.IsNotificationSent)
                        {
                            notificationSenderService.SendNotification(y => NotificationDefinitionRegistry.GetDefinition(y), notification);
                        }
                        else
                        {
                            Logger.Info("Skip {0} Veeva Client notifications because the parent {1} Veeva Client notification was not sent.", notification.Id, parentNotification.Id);
                        }
                    }
                }
                else
                {
                    notificationSenderService.SendNotification(y => NotificationDefinitionRegistry.GetDefinition(y), notification);
                }
            }

            Logger.Info("Finished sending Veeva Client notifications.");

            return Task.CompletedTask;
        }
    }
}