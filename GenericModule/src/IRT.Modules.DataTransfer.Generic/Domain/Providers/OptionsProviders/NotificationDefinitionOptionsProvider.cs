using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Frameworks.ExtendedProperties.Metadata;
using Frameworks.ExtendedProperties.ValueObjects;
using Frameworks.Notifications;
using IRT.Domain.Notifications;
using IRT.Domain.Services.Impl;
using Kernel.Utilities.Extensions;
using Unity;

namespace IRT.Modules.DataTransfer.Generic.Domain.Providers.OptionsProviders
{
    public class NotificationDefinitionOptionsProvider(IUnityContainer container)
        : IrtOptionsProvider
    {
        public override IEnumerable<ExtendedPropertyOption> GetOptions(
            EntityInfo entityInfo,
            ExtendedPropertyMetadata extendedPropertyMetadata)
        {
            NotificationDefinition[] allNotificationDefinitions = NotificationDefinitionRegistry.GetNotificationDefinitions(null);

            var notificationDefinitions = new List<NotificationDefinition>();

            foreach (var senderService in container.ResolveAll<INotificationSenderService>())
            {
                notificationDefinitions.AddRange(allNotificationDefinitions.Where(n => n.PendingNotificationSenderTypes.Contains(senderService.GetType())));
            }

            var options = notificationDefinitions
                .OrderBy(n => n.Name)
                .Select(x => new ExtendedPropertyOption
                {
                    Text = string.Equals(x.Name, Notifications.Resources.NotificationDefinitions.GenericRuntimeNotification)
                        ? Notifications.Resources.NotificationDefinitions.GenericRuntimeNotification.F(x.InvariantName)
                        : x.Name,
                    Value = x.Id.ToString(),
                });

            return options;
        }
    }
}