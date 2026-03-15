using System;
using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;
using IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices.Interfaces;
using Kernel.Plugins;

namespace IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices
{
    public class GenericNotificationDataServiceRepository : BaseServiceFactory<IGenericNotificationDataService>, IGenericNotificationDataServiceRepository
    {
        public GenericNotificationDataServiceRepository(IPluginsResolver pluginsResolver, IServiceProvider serviceProvider)
            : base(pluginsResolver, serviceProvider)
        {
        }
    }
}