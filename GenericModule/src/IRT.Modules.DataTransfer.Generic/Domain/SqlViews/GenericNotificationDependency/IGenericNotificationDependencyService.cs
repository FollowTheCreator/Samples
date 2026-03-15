using System;
using System.Collections.Generic;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDependency
{
    public interface IGenericNotificationDependencyService
    {
        NotificationDependencySqlView GetNotificationDependencyFor(Guid notificationId);

        NotificationDependencySqlView GetLastDependencyInGroup(string groupKey);

        Guid? EnqueueNotification(string groupKey, Guid notificationId);

        void RemoveNotification(Guid notificationId);

        Guid? EnqueueAfter(Guid notificationId, Guid dependsOnId, string groupKey);

        Guid? EnqueueBefore(Guid notificationId, Guid nextNotificationId, string groupKey);

        IEnumerable<NotificationDependencySqlView> GetForStartingNotification(Guid startingNotificationId);

        IEnumerable<NotificationDependencySqlView> GetForGroup(string groupKey);

        Guid? GetDependentNotificationId(Guid notificationId);

        bool Exists(Guid notificationId, out NotificationDependencySqlView notificationDependencySqlView);
    }
}
