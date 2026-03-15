using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;

namespace IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices.Interfaces
{
    // ToDo: Delete
    public interface IGenericNotificationDataServiceRepository : IServiceFactory
    {
        bool TryGetServiceInstance(string dataServiceName, out IGenericNotificationDataService providerInstance);
    }
}
