using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Frameworks.Notifications.Entities;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Events;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDefinition;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDependency;
using Kernel.Globalization.Constants;
using Kernel.Utilities;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.SqlViews
{
     public class ClientNotificationSqlViewHandler : IntegrationProcessSqlViewHandler
    {
        public ClientNotificationSqlViewHandler(
            IEventGenerationService eventGenerationService,
            IGenericNotificationDependencyService genericNotificationDependencyService,
            IGenericNotificationDefinitionService genericNotificationDefinitionService)
            : base(eventGenerationService, genericNotificationDependencyService, genericNotificationDefinitionService)
        {
        }

        public void Handle(ClientNotificationSent e)
        {
            var dbNotification = Db.Notifications.Find(e.ClientNotificationId);

            dbNotification.IsNotificationSent = true;
            dbNotification.SentUtcDateTime = e.SentUtcDateTime;

            Db.SaveChanges();
        }

        public void Handle(ClientNotificationAdditionalInfoUpdated e)
        {
            var dbNotification = Db.Notifications.Find(e.ClientNotificationId);

            dbNotification.AdditionalInfo = e.AdditionalInfo;

            Db.SaveChanges();
        }

        public void Handle(ClientNotificationUpdated e)
        {
            var notification = Db.Notifications.Where(x => x.Id == e.NotificationId).Include(x => x.NotificationLocalizedContentEntries).FirstOrDefault();

            notification.NotificationLocalizedContentEntries.ForEach(x =>
            {
                x.Title = e.Title;
                x.Body = e.Body;
            });

            notification.IsNotificationSent = e.IsNotificationSent;
            notification.FailedSendAttempts = e.FailedSendAttempts;

            Db.SaveChanges();
        }

        public void Handle(ClientNotificationCreated e)
        {
            var notification = new NotificationSqlView
            {
                Id = e.NotificationId,
                SubjectVisitId = e.SubjectVisitId,
                SubjectId = e.SubjectId,
                VisitId = e.VisitId,
                NotificationLocalizedContentEntries = new List<NotificationLocalizedContentSqlView>
                {
                    new NotificationLocalizedContentSqlView
                    {
                        Id = SequenticalGuid.NewGuid(),
                        NotificationId = e.NotificationId,
                        LanguageId = GlobalizationConstants.DefaultLanguage,
                        Title = e.Title,
                        Body = e.Body,
                        GeneratedUtcDateTime = e.TransactionUtcDateTime
                    }
                },
                NotificationDefinitionId = e.NotificationDefinitionId,
                SiteId = e.SiteId,
                GeneratedLocalDate = e.TransactionLocalDateTime,
                GeneratedUtcDateTime = e.TransactionUtcDateTime
            };

            Db.Notifications.Add(notification);
            Db.SaveChanges();

            EnqueueNotification(notification.Id, e.NotificationDefinitionId, e.DependentNotificationId, e.SubjectId);
        }

        public void Handle(NotificationMarkedAsSent e)
        {
            e.NotificationIds.ForEach(notificationId =>
            {
                Db.Notifications
                 .Where(x => x.Id == notificationId && !x.IsNotificationSent)
                 .ExecuteUpdate(set => set.SetProperty(n => n.IsNotificationSent, true));
            });
        }

        private void EnqueueNotification(
            Guid notificationId,
            Guid notificationDefinitionId,
            Guid? dependentNotificationId,
            Guid? subjectId)
        {
            if (!genericNotificationDefinitionService.GetIdsFor(NotificationVendor.Veeva).Contains(notificationDefinitionId))
            {
                return;
            }

            if (dependentNotificationId != null)
            {
                var notificationDependency = Db.Set<NotificationDependencySqlView>()
                    .FirstOrDefault(g => g.NotificationId == dependentNotificationId);

                if (notificationDependency != null)
                {
                    genericNotificationDependencyService.EnqueueNotification(
                        notificationDependency.GroupKey,
                        notificationId);
                }
                else
                {
                    genericNotificationDependencyService.EnqueueNotification(
                        subjectId.ToString(),
                        notificationId);
                }
            }
            else
            {
                genericNotificationDependencyService.EnqueueNotification(subjectId.ToString(), notificationId);
            }

            Db.SaveChanges();
        }
    }
}
