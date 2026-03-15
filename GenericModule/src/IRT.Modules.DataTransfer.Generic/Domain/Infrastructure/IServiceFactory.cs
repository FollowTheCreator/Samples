using System.Collections.Generic;
using IRT.Modules.DataTransfer.Generic.Domain.Providers.Models;

namespace IRT.Modules.DataTransfer.Generic.Domain.Infrastructure
{
    public interface IServiceFactory
    {
        IEnumerable<ServiceMetadata> GetServices();
    }
}
