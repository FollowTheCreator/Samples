using IRT.Domain;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces
{
    public interface IContextsGeneratorFactory
    {
        IContextsGenerator Create(IRTEvent irtEvent);
    }
}