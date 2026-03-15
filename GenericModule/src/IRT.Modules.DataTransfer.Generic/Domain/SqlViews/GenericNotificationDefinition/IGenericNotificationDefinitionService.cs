using System;
using System.Collections.Generic;
using Frameworks.Notifications;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDefinition
{
    public interface IGenericNotificationDefinitionService
    {
        List<Guid> GetIdsFor(NotificationVendor notificationVendor);

        List<Guid> GetIds();

        List<NotificationDefinition> GetNotificationDefinitions();

        List<NotificationDefinition> GetNotificationDefinitions(NotificationVendor notificationVendor);
    }
}