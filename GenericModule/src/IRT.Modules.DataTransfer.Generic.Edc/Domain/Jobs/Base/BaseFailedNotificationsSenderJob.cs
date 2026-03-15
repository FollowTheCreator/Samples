using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NLog;
using Quartz;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications;
using Frameworks.Notifications.Entities;
using IRT.Domain.Notifications;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands.Rave;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs.Resources;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.Errors;
using Kernel.DDD.Dispatching;
using Kernel.Jobs.Jobs;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs.Base
{
    [DisallowConcurrentExecution]
    public abstract class BaseFailedNotificationsSenderJob : BaseJob
    {
        protected readonly ICommandBus CommandBus;
        protected readonly IExtendedPropertiesValueProvider ExtendedPropertiesValueProvider;
        protected readonly IQueryable<NotificationSqlView> NotificationsQuery;
        protected readonly INotificationSenderService NotificationSenderService;

        public BaseFailedNotificationsSenderJob(
           ICommandBus commandBus,
           IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
           IQueryable<NotificationSqlView> notificationsQuery,
           INotificationSenderService notificationSenderService)
        {
            CommandBus = commandBus;
            ExtendedPropertiesValueProvider = extendedPropertiesValueProvider;
            NotificationsQuery = notificationsQuery;
            NotificationSenderService = notificationSenderService;
        }

        public override Task ExecuteInternal(IJobExecutionContext context)
        {
            if (!CanSendNotifications(context))
            {
                Logger.Info(FailedSendingJobResources.LocalizedMessage_DataTransferSendingIsNotEnabled);
                return Task.CompletedTask;
            }

            SendFailedNotifications(context);

            SendFailedNotificationsSummary(context);

            return Task.CompletedTask;
        }

        protected virtual string GetDataTransferName(IJobExecutionContext context)
        {
            var assembly = NotificationSenderService.GetType().Assembly;

            return assembly
                .GetCustomAttributes<AssemblyTitleAttribute>()
                .Select(x => x.Title)
                .FirstOrDefault()
                ?? assembly.GetName().Name;
        }

        protected virtual IQueryable<NotificationSqlView> GetFailedNotificationsQuery(IJobExecutionContext context)
        {
            return NotificationSenderService
                .FilterRetransmitNotifications(NotificationsQuery, NotificationSenderService.GetType().FullName)
                .Include(x => x.NotificationLocalizedContentEntries)
                .Where(x => !x.IsNotificationSent && x.AdditionalInfo != null);
        }

        protected virtual void SendFailedNotifications(IJobExecutionContext context)
        {
            var dataTransferName = GetDataTransferName(context);

            Logger.Info(FailedSendingJobResources.LocalizedMessage_StartedSendingNotifications, dataTransferName);

            var failedNotifications = FilterFailedNotifications(
                context,
                GetFailedNotificationsQuery(context).AsEnumerable())
                .ToList();

            Logger.Info(FailedSendingJobResources.LocalizedMessage_SendingCount, failedNotifications.Count, dataTransferName);

            failedNotifications.ForEach(x => NotificationSenderService.SendNotification(
                _ => NotificationDefinitionRegistry.GetDefinition(_), x));

            Logger.Info(FailedSendingJobResources.LocalizedMessage_FinishedSendingNotifications, dataTransferName);
        }

        protected virtual void SendFailedNotificationsSummary(IJobExecutionContext context)
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
                // TODO: need a better way to do this
                var result = CommandBus.SendCommand(new CreateRaveDataTransferErrorSummaryNotification
                {
                    NotificationId = Guid.NewGuid(),
                    NotificationDefinitionId =
                        Notifications.NotificationDefinitions.RaveErrorsListNotification.Id,
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

        protected abstract bool CanSendNotifications(IJobExecutionContext context);

        protected abstract IEnumerable<NotificationSqlView> FilterFailedNotifications(
            IJobExecutionContext context, IEnumerable<NotificationSqlView> notifications);
    }
}