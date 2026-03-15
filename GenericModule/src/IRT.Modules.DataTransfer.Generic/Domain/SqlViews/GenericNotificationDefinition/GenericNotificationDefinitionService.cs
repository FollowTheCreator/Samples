using System;
using System.Collections.Generic;
using System.Linq;
using Frameworks.Notifications;
using IRT.Domain.Notifications;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces.Clients;
using Unity;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDefinition
{
    public class GenericNotificationDefinitionService : IGenericNotificationDefinitionService
    {
        private readonly IUnityContainer container;

        public GenericNotificationDefinitionService(IUnityContainer container)
        {
            this.container = container;
        }

        public List<NotificationDefinition> GetNotificationDefinitions()
        {
            var allNotificationDefinitions = NotificationDefinitionRegistry.GetNotificationDefinitions(null);
            var notificationDefinitions = new List<NotificationDefinition>();

            foreach (var senderService in container.ResolveAll<INotificationSenderService>())
            {
                notificationDefinitions.AddRange(allNotificationDefinitions.Where(n => n.PendingNotificationSenderTypes.Contains(senderService.GetType())));
            }

            return notificationDefinitions;
        }

        public List<NotificationDefinition> GetNotificationDefinitions(NotificationVendor notificationVendor)
        {
            var allNotificationDefinitions = NotificationDefinitionRegistry.GetNotificationDefinitions(null);
            var notificationDefinitions = new List<NotificationDefinition>();

            foreach (var senderService in container.ResolveAll<INotificationSenderService>())
            {
                switch (notificationVendor)
                {
                    case NotificationVendor.Rave:
                        if (typeof(IRaveWebApiNotificationSenderService).IsAssignableFrom(senderService.GetType()))
                        {
                            notificationDefinitions.AddRange(
                                allNotificationDefinitions.Where(n => n.PendingNotificationSenderTypes.Contains(senderService.GetType())));
                        }

                        break;
                    case NotificationVendor.Veeva:
                        if (typeof(IVeevaWebApiNotificationSenderService).IsAssignableFrom(senderService.GetType()))
                        {
                            notificationDefinitions.AddRange(
                                allNotificationDefinitions.Where(n => n.PendingNotificationSenderTypes.Contains(senderService.GetType())));
                        }

                        break;
                    default:
                        notificationDefinitions.AddRange(
                                allNotificationDefinitions.Where(n => n.PendingNotificationSenderTypes.Contains(senderService.GetType())));
                        break;
                }
            }

            return notificationDefinitions;
        }

        public List<Guid> GetIds()
        {
            var clientNotificationDefinitionIds = GetNotificationDefinitions()
                            .Select(g => g.Id)
                            .ToList();
            return clientNotificationDefinitionIds;
        }

        public List<Guid> GetIdsFor(NotificationVendor notificationVendor)
        {
            var clientNotificationDefinitionIds = GetNotificationDefinitions(notificationVendor)
                            .Select(g => g.Id)
                            .ToList();

            return clientNotificationDefinitionIds;
        }
    }
}