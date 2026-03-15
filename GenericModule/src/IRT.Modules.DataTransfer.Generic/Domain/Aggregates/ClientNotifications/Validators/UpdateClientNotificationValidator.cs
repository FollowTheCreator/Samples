using System.Linq;
using Frameworks.Notifications.Entities;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands;
using IRT.Modules.DataTransfer.Generic.Helpers.Extensions;
using Kernel.DDD.Dispatching.Exceptions;
using Kernel.DDD.Domain.Validators;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Validators
{
    public class UpdateClientNotificationValidator : CommandValidatorBase<UpdateClientNotification>
    {
        private readonly IQueryable<NotificationSqlView> notificationsQuery;

        public UpdateClientNotificationValidator(IQueryable<NotificationSqlView> notificationsQuery)
        {
            this.notificationsQuery = notificationsQuery;
        }

        protected override void Validate(UpdateClientNotification c)
        {
            var notification = notificationsQuery.SingleOrDefault(x => x.Id == c.NotificationId);
            if (notification is null)
            {
                throw new CommandValidationException(Resources.ValidationResources.NotificationNotFound);
            }

            if (c.FailedSendAttempts < 0)
            {
                throw new CommandValidationException(Resources.ValidationResources.FailedAtteptsError);
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
