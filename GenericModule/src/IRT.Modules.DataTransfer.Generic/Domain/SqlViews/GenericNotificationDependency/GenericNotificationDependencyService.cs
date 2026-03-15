using System;
using System.Collections.Generic;
using System.Linq;
using IRT.Domain;
using Kernel.Infrastructure.DateTimeProvider;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDependency
{
    public class GenericNotificationDependencyService : IGenericNotificationDependencyService
    {
        private readonly IRTDbContext db;
        private readonly IDateTimeService dateTimeService;

        public GenericNotificationDependencyService(IRTDbContext db, IDateTimeService dateTimeService)
        {
            this.db = db;
            this.dateTimeService = dateTimeService;
        }

        public NotificationDependencySqlView GetNotificationDependencyFor(Guid notificationId)
        {
            var genericNotificationDependency = db.Set<NotificationDependencySqlView>()
                .FirstOrDefault(n => n.NotificationId == notificationId);

            return genericNotificationDependency;
        }

        public NotificationDependencySqlView GetLastDependencyInGroup(string groupKey)
        {
            var result = db.Set<NotificationDependencySqlView>()
                .Where(x => x.GroupKey == groupKey)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            return result;
        }

        public IEnumerable<NotificationDependencySqlView> GetForGroup(string groupKey)
        {
            var dependencies = db.Set<NotificationDependencySqlView>()
                .Where(x => x.GroupKey == groupKey);

            var root = dependencies.Single(n => n.DependsOnId == null);

            yield return root;

            var current = root;

            while ((current = dependencies.FirstOrDefault(x => x.DependsOnId == current.NotificationId)) != null)
            {
                yield return current;
            }
        }

        public Guid? EnqueueNotification(string groupKey, Guid notificationId)
        {
            if (IsEnqueued(notificationId, out NotificationDependencySqlView existingDependency))
            {
                return existingDependency.DependsOnId;
            }

            var lastDependency = GetLastDependencyInGroup(groupKey);
            if (lastDependency == null)
            {
                var dependency = CreateDependency(notificationId, null, groupKey);
                db.Set<NotificationDependencySqlView>().Add(dependency);
                db.SaveChanges();
                return null;
            }
            else
            {
                EnqueueAfter(notificationId, lastDependency.NotificationId, groupKey);
                return lastDependency.NotificationId;
            }
        }

        public void RemoveNotification(Guid notificationId)
        {
            var existingDependency = db.Set<NotificationDependencySqlView>()
                .FirstOrDefault(x => x.NotificationId == notificationId);

            if (existingDependency == null)
            {
                return;
            }

            db.Set<NotificationDependencySqlView>().Remove(existingDependency);

            var childNotification = GetNext(existingDependency.NotificationId);
            if (childNotification != null)
            {
                childNotification.DependsOnId = existingDependency.DependsOnId;
            }

            db.SaveChanges();
        }

        public Guid? EnqueueAfter(Guid notificationId, Guid dependsOnId, string groupKey)
        {
            NotificationDependencySqlView dependency;
            if (IsEnqueued(notificationId, out dependency))
            {
                dependency.DependsOnId = dependsOnId;
            }
            else
            {
                dependency = CreateDependency(notificationId, dependsOnId, groupKey);
                db.Set<NotificationDependencySqlView>().Add(dependency);
            }

            var childNotification = GetNext(dependsOnId);
            if (childNotification != null)
            {
                childNotification.DependsOnId = dependency.NotificationId;
            }

            db.SaveChanges();
            return dependsOnId;
        }

        public Guid? EnqueueBefore(Guid notificationId, Guid nextNotificationId, string groupKey)
        {
            NotificationDependencySqlView nextNotificationDependency;
            var currentNotificationDependsOnId = IsEnqueued(nextNotificationId, out nextNotificationDependency)
                ? nextNotificationDependency.DependsOnId
                : Guid.Empty;

            NotificationDependencySqlView currentNotificationDependency;
            if (!IsEnqueued(notificationId, out currentNotificationDependency))
            {
                currentNotificationDependency = CreateDependency(notificationId, currentNotificationDependsOnId, groupKey);
                db.Set<NotificationDependencySqlView>().Add(currentNotificationDependency);
            }
            else
            {
                currentNotificationDependency.DependsOnId = currentNotificationDependsOnId;
            }

            if (nextNotificationDependency != null)
            {
                nextNotificationDependency.DependsOnId = currentNotificationDependency.NotificationId;
            }

            db.SaveChanges();
            return currentNotificationDependsOnId;
        }

        public IEnumerable<NotificationDependencySqlView> GetForStartingNotification(Guid startingNotificationId)
        {
            NotificationDependencySqlView next;
            var currentNotificationId = startingNotificationId;

            while ((next = GetNext(currentNotificationId)) != null)
            {
                currentNotificationId = next.NotificationId;
                yield return next;
            }
        }

        public Guid? GetDependentNotificationId(Guid notificationId)
        {
            var result = db.Set<NotificationDependencySqlView>()
                .FirstOrDefault(x => x.NotificationId == notificationId);
            var dependentId = result?.DependsOnId;

            return dependentId;
        }

        public bool Exists(Guid notificationId, out NotificationDependencySqlView notificationDependencySqlView)
        {
            notificationDependencySqlView = db.Set<NotificationDependencySqlView>()
                .FirstOrDefault(x => x.NotificationId == notificationId);

            return notificationDependencySqlView != null;
        }

        private NotificationDependencySqlView CreateDependency(Guid notificationId, Guid? dependsOnId, string groupKey)
        {
            var dependency = new NotificationDependencySqlView
            {
                NotificationId = notificationId,
                DependsOnId = dependsOnId,
                GroupKey = groupKey,
                CreatedAt = dateTimeService.GetCurrentUtcDateTime()
            };

            return dependency;
        }

        private bool IsEnqueued(Guid notificationId, out NotificationDependencySqlView dependency)
        {
            dependency = db.Set<NotificationDependencySqlView>()
                .FirstOrDefault(x => x.NotificationId == notificationId);

            var result = dependency != null;
            return result;
        }

        private NotificationDependencySqlView GetNext(Guid notificationId)
        {
            var result = db.Set<NotificationDependencySqlView>()
                .FirstOrDefault(x => x.DependsOnId == notificationId);

            return result;
        }
    }
}