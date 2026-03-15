using System.Linq;
using Microsoft.EntityFrameworkCore;
using IRT.Domain.ViewsSql.Site;
using IRT.Domain.ViewsSql.Subject;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands;
using Kernel.DDD.Dispatching;
using Kernel.Infrastructure.DateTimeProvider;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications
{
    public class ClientNotificationCommandHandler : ICommandHandler
    {
        private readonly AggregateRepository<ClientNotificationAggregate> aggregateRepository;
        private readonly IDateTimeService dateTimeService;
        private readonly IQueryable<SubjectSqlView> subjectsQuery;
        private readonly IQueryable<SiteSqlView> sitesQuery;

        public ClientNotificationCommandHandler(
            AggregateRepository<ClientNotificationAggregate> aggregateRepository,
            IDateTimeService dateTimeService,
            IQueryable<SubjectSqlView> subjectsQuery,
            IQueryable<SiteSqlView> sitesQuery)
        {
            this.aggregateRepository = aggregateRepository;
            this.dateTimeService = dateTimeService;
            this.subjectsQuery = subjectsQuery;
            this.sitesQuery = sitesQuery;
        }

        public void Handle(AcknowledgeClientNotification command)
        {
            if (command.IsSuccess)
            {
                var dateTimeUtcNow = dateTimeService.GetCurrentUtcDateTime();

                aggregateRepository.Perform(
                    aggregateId: command.ClientNotificationId,
                    action: a => a.ClientNotificationSent(command, dateTimeUtcNow));
            }
            else
            {
                aggregateRepository.Perform(
                    aggregateId: command.ClientNotificationId,
                    action: a => a.UpdateClientNotificationAdditionalInfo(command));
            }
        }

        public void Handle(ReceiveClientNotification command)
        {
            aggregateRepository.Perform(
                    aggregateId: NotificationManagerId,
                    action: a => a.ReceiveClientNotification(command: command));
        }

        public void Handle(UpdateClientNotification command)
        {
            var site = sitesQuery.Where(x => x.SiteId == command.SiteId).SingleOrDefault();

            aggregateRepository.Perform(
                    aggregateId: command.NotificationId,
                    action: a => a.UpdateClientNotification(
                        command: command,
                        localDateTime: dateTimeService.GetCurrentLocalDateTime(site),
                        currentUTCTime: dateTimeService.GetCurrentUtcDateTime()));
        }

        public void Handle(CreateClientNotification command)
        {
            var subjectInfo = subjectsQuery.FirstOrDefault(x => x.SubjectId == command.SubjectId);
            var site = sitesQuery.Where(x => x.SiteId == subjectInfo.SiteId).Include(x => x.DefaultLocation).FirstOrDefault();

            var currentUTCTime = dateTimeService.GetCurrentUtcDateTime();
            var localDateTime = dateTimeService.GetLocalDateTime(currentUTCTime, site);

            aggregateRepository.Perform(
                    aggregateId: command.NotificationId,
                    action: a => a.CreateClientNotification(
                        command: command,
                        siteId: site.SiteId,
                        localDateTime: localDateTime,
                        currentUTCTime: currentUTCTime));
        }

        public void Handle(MarkNotificationsAsSent command)
        {
            aggregateRepository.Perform(
                    aggregateId: NotificationManagerId,
                    action: a => a.MarkNotificationsAsSent(command));
        }

        public static string NotificationManagerId => "__NotificationManager";
    }
}
