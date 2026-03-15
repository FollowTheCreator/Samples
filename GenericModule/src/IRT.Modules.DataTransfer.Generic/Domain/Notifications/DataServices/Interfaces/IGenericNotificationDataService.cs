using Frameworks.Notifications.Entities;
using Kernel.DDD.Domain.Events;

namespace IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices.Interfaces
{
    public interface IGenericNotificationDataService
    {
        public void MapModelDataInternal(NotificationSqlView notification, Event e);
    }
}
