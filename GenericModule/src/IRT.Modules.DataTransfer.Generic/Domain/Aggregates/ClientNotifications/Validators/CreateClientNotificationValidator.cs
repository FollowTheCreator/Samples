using System.Linq;
using Microsoft.EntityFrameworkCore;
using Frameworks.Notifications.Entities;
using IRT.Domain.ViewsSql.Subject;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands;
using IRT.Modules.DataTransfer.Generic.Helpers.Extensions;
using Kernel.DDD.Dispatching.Exceptions;
using Kernel.DDD.Domain.Validators;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Validators
{
    public class CreateClientNotificationValidator : CommandValidatorBase<CreateClientNotification>
    {
        private readonly IQueryable<NotificationSqlView> notificationsQuery;
        private readonly IQueryable<SubjectSqlView> subjectsQuery;
        private readonly IQueryable<SubjectVisitSqlView> subjectVisitsQuery;
        private readonly IQueryable<NotificationDefinitionSqlView> notificationDefinitionsQuery;

        public CreateClientNotificationValidator(
            IQueryable<NotificationSqlView> notificationsQuery,
            IQueryable<SubjectSqlView> subjectsQuery,
            IQueryable<SubjectVisitSqlView> subjectVisitsQuery,
            IQueryable<NotificationDefinitionSqlView> notificationDefinitionsQuery)
        {
            this.notificationsQuery = notificationsQuery;
            this.subjectsQuery = subjectsQuery;
            this.subjectVisitsQuery = subjectVisitsQuery;
            this.notificationDefinitionsQuery = notificationDefinitionsQuery;
        }

        protected override void Validate(CreateClientNotification c)
        {
            if (notificationsQuery.Any(x => x.Id == c.NotificationId))
            {
                throw new CommandValidationException(Resources.ValidationResources.SameID);
            }

            var subject = subjectsQuery.AsNoTracking().SingleOrDefault(x => x.SubjectId == c.SubjectId);
            if (subject is null)
            {
                throw new CommandValidationException(Resources.ValidationResources.SubjectNotFound);
            }

            var subjectVisit = c.SubjectVisitId.HasValue
                ? subjectVisitsQuery.AsNoTracking().SingleOrDefault(x => x.Id == c.SubjectVisitId.Value)
                : null;
            if (c.SubjectVisitId.HasValue && subjectVisit is null)
            {
                throw new CommandValidationException(Resources.ValidationResources.SubjectVisitNotFound);
            }

            if (subjectVisit != null && c.SubjectId != subjectVisit.SubjectId)
            {
                throw new CommandValidationException(Resources.ValidationResources.SubjectDoesntMatch);
            }

            if (!notificationDefinitionsQuery.Any(x => x.NotificationTypeId == c.NotificationDefinitionId))
            {
                throw new CommandValidationException(Resources.ValidationResources.NotificationNotFound);
            }

            if (string.IsNullOrWhiteSpace(c.Title))
            {
                throw new CommandValidationException(Resources.ValidationResources.EmptyTitle);
            }

            if (string.IsNullOrWhiteSpace(c.Body))
            {
                throw new CommandValidationException(Resources.ValidationResources.EmptyBody);
            }

            if (!c.Body.CanFormatToXml() && !c.Body.CanFormatToJson())
            {
                throw new CommandValidationException(Resources.ValidationResources.InvalidBody);
            }
        }
    }
}
