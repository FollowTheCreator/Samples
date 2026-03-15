using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces
{
    public interface IContextsGenerator
    {
        BaseGenerationContext GetGenerationContext(IRTEvent irtEvent);
    }
}