using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain.ViewsSql.Study;
using IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Models;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Events.Rave;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;
using Kernel.DDD.Dispatching;
using Kernel.Globalization.Constants;
using Kernel.Infrastructure.DateTimeProvider;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands.Rave
{
    public class RaveNotificationCommandHandler : ICommandHandler
    {
        private readonly AggregateRepository<ClientNotificationAggregate> notificationAggregateRepository;
        private readonly AggregateRepository<RaveNotificationAggregate> raveAggregateRepository;
        private readonly IDateTimeService dateTimeService;
        private readonly IQueryable<NotificationSqlView> notificationsQuery;
        private readonly IExtendedPropertiesValueProvider extendedPropertiesValueProvider;

        public RaveNotificationCommandHandler(
            AggregateRepository<ClientNotificationAggregate> notificationAggregateRepository,
            AggregateRepository<RaveNotificationAggregate> raveAggregateRepository,
            IDateTimeService dateTimeService,
            IQueryable<NotificationSqlView> notificationsQuery,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider)
        {
            this.notificationAggregateRepository = notificationAggregateRepository;
            this.raveAggregateRepository = raveAggregateRepository;
            this.dateTimeService = dateTimeService;
            this.notificationsQuery = notificationsQuery;
            this.extendedPropertiesValueProvider = extendedPropertiesValueProvider;
        }

        public void Handle(RaveAknowledgeClientNotification command)
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

                string postedMessage = string.Empty;

                // First try to get the updated localized content
                postedMessage = clientNotification.NotificationLocalizedContentEntries
                    .Where(z => z.LanguageId == GlobalizationConstants.DefaultLanguage)
                    .OrderByDescending(z => z.GeneratedUtcDateTime)
                    .Select(z => z.Body)
                    .FirstOrDefault();

                // Only fall back to Model if LocalizedContent is not available
                if (postedMessage.IsNullOrEmpty() && clientNotification.Model != null)
                {
                    postedMessage = JsonConvert.DeserializeObject<ClientNotificationViewModel>(clientNotification.Model)?.Body;
                }

                // TODO: read this from study settings
                var notificationTitle = "Client Rave Error Notification";

                var studySettings = extendedPropertiesValueProvider.GetValue<StudySqlView, GenericEdcDataTransferStudySettings>(null);
                if (studySettings.EnableRaveErrorNotification)
                {
                    // Create Rave Client Error Notification
                    raveAggregateRepository.Perform(
                    aggregateId: command.ClientNotificationId,
                    action: a => a.CreateRaveClientErrorNotification(
                        command: command,
                        postedMessage: postedMessage,
                        notificationTitle: notificationTitle));
                }

                notificationAggregateRepository.Perform(
                    aggregateId: command.ClientNotificationId,
                    action: a => a.UpdateClientNotificationAdditionalInfo(command));
            }
        }

        public void Handle(CreateRaveDataTransferErrorSummaryNotification command)
        {
            raveAggregateRepository.Perform(
                command.NotificationId,
                a => a.AddDomainEvent(new RaveDataTransferErrorSummaryNotificationCreated
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
