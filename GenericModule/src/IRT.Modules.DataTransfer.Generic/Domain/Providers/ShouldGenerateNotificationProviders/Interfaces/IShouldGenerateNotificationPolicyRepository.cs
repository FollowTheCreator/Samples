using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;

namespace IRT.Modules.DataTransfer.Generic.Domain.Providers.ShouldGenerateNotificationProviders.Interfaces
{
    public interface IShouldGenerateNotificationPolicyRepository : IServiceFactory
    {
        bool TryGetServiceInstance(string type, out IShouldGenerateNotificationPolicy shouldGenerateNotificationProvider);
    }
}
