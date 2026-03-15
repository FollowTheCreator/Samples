using System.Collections.Generic;
using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.GenericDataGeneration.Events;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces
{
    public interface IEventGenerationService
    {
        public IEnumerable<GenericIntegrationDataEvent> CreateGenericDataEvent(IRTEvent irtEvent);
    }
}
