using System;
using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;
using IRT.Modules.DataTransfer.Generic.Domain.Providers.ShouldGenerateNotificationProviders.Interfaces;
using Kernel.Plugins;

namespace IRT.Modules.DataTransfer.Generic.Domain.Providers.ShouldGenerateNotificationProviders
{
    public class ShouldGenerateNotificationPolicyRepository
        : BaseServiceFactory<IShouldGenerateNotificationPolicy>, IShouldGenerateNotificationPolicyRepository
    {
        public ShouldGenerateNotificationPolicyRepository(IPluginsResolver pluginResolver, IServiceProvider serviceProvider)
            : base(pluginResolver, serviceProvider)
        {
        }
    }
}
