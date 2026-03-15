using System.Linq;
using Microsoft.EntityFrameworkCore;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain.ViewsSql.Study;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Events.Veeva;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;
using Kernel.DDD.Dispatching;
using Kernel.Globalization.Constants;
using Kernel.Infrastructure.DateTimeProvider;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands.Veeva
{
    public class VeevaNotificationCommandHandler : ICommandHandler
    {
        private readonly AggregateRepository<ClientNotificationAggregate> notificationAggregateRepository;
        private readonly AggregateRepository<VeevaNotificationAggregate> veevaAggregateRepository;
        private readonly IDateTimeService dateTimeService;
        private readonly IQueryable<NotificationSqlView> notificationsQuery;
        private readonly IExtendedPropertiesValueProvider extendedPropertiesValueProvider;

        public VeevaNotificationCommandHandler(
            AggregateRepository<ClientNotificationAggregate> notificationAggregateRepository,
            AggregateRepository<VeevaNotificationAggregate> veevaAggregateRepository,
            IDateTimeService dateTimeService,
            IQueryable<NotificationSqlView> notificationsQuery,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider)
        {
            this.notificationAggregateRepository = notificationAggregateRepository;
            this.veevaAggregateRepository = veevaAggregateRepository;
            this.dateTimeService = dateTimeService;
            this.notificationsQuery = notificationsQuery;
            this.extendedPropertiesValueProvider = extendedPropertiesValueProvider;
        }

        public void Handle(VeevaAcknowledgeClientNotification command)
        {
            if (command.IsSuccess)
            {
                var dateTimeUtcNow = dateTimeService.GetCurrentUtcDateTime();

                notificationAggregateRepository.Perform(
                    aggregateId: command.ClientNotificationId,
                    action: a => a.ClientNotificationSent(command, dateTimeUtcNow));
            }
            else
            {
                var clientNotification = notificationsQuery
                    .Include(x => x.NotificationLocalizedContentEntries)
                    .First(x => x.Id == command.ClientNotificationId);

                string postedMessage;
                //if (clientNotification.Model != null)
                //{
                //    // need to fix this
                //    postedMessage = JsonConvert.DeserializeObject<ClientNotificationViewModel>(clientNotification.Model)?.Body;
                //}
                //else
                //{
                    postedMessage = clientNotification.NotificationLocalizedContentEntries
                        .Where(z => z.LanguageId == GlobalizationConstants.DefaultLanguage)
                    .OrderByDescending(z => z.GeneratedUtcDateTime)
                        .Select(z => z.Body)
                        .FirstOrDefault();
                //}

                // TODO: read this from study settings
                var notificationTitle = "Client Veeva Error Notification";

                var studySettings = extendedPropertiesValueProvider.GetValue<StudySqlView, GenericEdcDataTransferStudySettings>(null);
                if (studySettings.EnableVeevaErrorNotification)
                {
                    // Create Veeva Client Error Notification
                    veevaAggregateRepository.Perform(
                    aggregateId: command.ClientNotificationId,
                    action: a => a.CreateVeevaClientErrorNotification(
                        command: command,
                        postedMessage: postedMessage,
                        notificationTitle: notificationTitle));
                }

                notificationAggregateRepository.Perform(
                    aggregateId: command.ClientNotificationId,
                    action: a => a.UpdateClientNotificationAdditionalInfo(command));
            }
        }

        public void Handle(CreateVeevaDataTransferErrorSummaryNotification command)
        {
            veevaAggregateRepository.Perform(
                command.NotificationId,
                a => a.AddDomainEvent(new VeevaDataTransferErrorSummaryNotificationCreated
                {
                    NotificationId = command.NotificationId,
                    NotificationDefinitionId = command.NotificationDefinitionId,
                    FailedNotifcations = command.FailedNotifications,
                    TransactionLocalDateTime = this.dateTimeService.GetCurrentUtcDateTime(),
                    TransactionUtcDateTime = this.dateTimeService.GetCurrentUtcDateTime()
                }));
        }

        public static string NotificationManagerId => "__NotificationManager";
    }
}
