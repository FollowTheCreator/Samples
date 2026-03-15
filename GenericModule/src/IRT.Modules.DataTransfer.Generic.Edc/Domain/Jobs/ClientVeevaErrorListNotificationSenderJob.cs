using System;
using System.Collections.Generic;
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
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands.Veeva;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs.Base;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs.Resources;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Services.Implementations;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.Errors;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;
using Kernel.DDD.Dispatching;
using Kernel.Globalization.Attributes;
using Kernel.Logging.Common;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs
{
    [LocalizedCategory(Resources.ClientVeevaErrorListNotificationGeneratorJob.ResourceNames.DataTransferGeneric, typeof(Resources.ClientVeevaErrorListNotificationGeneratorJob))]
    [LocalizedDisplayName(Resources.ClientVeevaErrorListNotificationGeneratorJob.ResourceNames.DisplayName, typeof(Resources.ClientVeevaErrorListNotificationGeneratorJob))]
    [LocalizedDescription(Resources.ClientVeevaErrorListNotificationGeneratorJob.ResourceNames.Description, typeof(Resources.ClientVeevaErrorListNotificationGeneratorJob))]
    [Logs(Edc.Domain.Operations.GenericDataTransfer.ViewVeevaJobsLogs)]
    public class ClientVeevaErrorListNotificationSenderJob : BaseFailedNotificationsSenderJob
    {
        private readonly IExtendedPropertiesValueProvider extendedPropertiesValueProvider;
        private readonly RaveWebApiNotificationSenderService notificationSenderService;
        private readonly IQueryable<NotificationSqlView> notificationsQuery;
        private readonly IGenericNotificationDefinitionService genericNotificationDefinitionService;
        private readonly ICommandBus commandBus;

        public ClientVeevaErrorListNotificationSenderJob(
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            RaveWebApiNotificationSenderService notificationSenderService,
            IQueryable<NotificationSqlView> notificationsQuery,
            IGenericNotificationDefinitionService genericNotificationDefinitionService,
            ICommandBus commandBus)
            //: base(notificationsQuery, notificationSenderService)
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
            if (!studySettings.EnableVeevaErrorNotification)
            {
                Logger.Warn("The Veeva Error Notifications are not enabled for this study");
                return false;
            }

            return true;
        }


        protected override IQueryable<NotificationSqlView> GetFailedNotificationsQuery(IJobExecutionContext context)
        {
            return NotificationsQuery
                .Include(x => x.NotificationDefinition)
                .Where(x => genericNotificationDefinitionService.GetIdsFor(NotificationVendor.Veeva).Contains(x.NotificationDefinitionId))
                .Include(x => x.NotificationLocalizedContentEntries)
                .Where(x => !x.IsNotificationSent && x.AdditionalInfo != null)
                .OrderBy(x => x.GeneratedUtcDateTime);
        }

        protected override void SendFailedNotifications(IJobExecutionContext context)
        {
            this.Logger.Info(VeevaClientSending.Sending_Started);

            var notifications = this.NotificationSenderService
                .FilterPendingNotifications(this.NotificationsQuery)
                .Include(x => x.NotificationDefinition)
                .Include(x => x.NotificationLocalizedContentEntries)
                .OrderBy(x => x.GeneratedUtcDateTime)
                .ToArray();
            this.Logger.Info(VeevaClientSending.Sending_Attempting, notifications.Length);

            notifications.ForEach(x => NotificationSenderService
                .SendNotification(NotificationDefinitionRegistry.GetDefinition, x));

            this.Logger.Info(VeevaClientSending.Sending_Finished);
        }

        // need a better way to do this
        protected override void SendFailedNotificationsSummary(IJobExecutionContext context)
        {
            var dataTransferName = GetDataTransferName(context);

            Logger.Info(FailedSendingJobResources.LocalizedMessage_SummaryStarted, dataTransferName);

            var failedNotifications = FilterFailedNotifications(
                context,
                GetFailedNotificationsQuery(context).AsEnumerable())
                .ToList();

            Logger.Info(FailedSendingJobResources.LocalizedMessage_SummaryCount, failedNotifications.Count, dataTransferName);

            if (failedNotifications.Any())
            {
                // need a better way to do this
                var result = CommandBus.SendCommand(new CreateVeevaDataTransferErrorSummaryNotification
                {
                    NotificationId = Guid.NewGuid(),
                    NotificationDefinitionId =
                        Notifications.NotificationDefinitions.VeevaErrorsListNotification.Id,
                    FailedNotifications = failedNotifications.Select(x =>
                        new ErrorListData(
                            x.NotificationLocalizedContentEntries.First().Title,
                            x.Id,
                            x.AdditionalInfo))
                        .ToArray()
                });
            }

            Logger.Info(FailedSendingJobResources.LocalizedMessage_SummaryCompleted, dataTransferName);
        }

        protected override IEnumerable<NotificationSqlView> FilterFailedNotifications(
            IJobExecutionContext context, IEnumerable<NotificationSqlView> notifications)
            => notifications;
    }
}