using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations
{
    public class ContextsGeneratorFactory : IContextsGeneratorFactory
    {
        protected readonly IContextsGeneratorRegistrationService Registry;

        public ContextsGeneratorFactory(IContextsGeneratorRegistrationService registry)
        {
            Registry = registry;
        }

        public virtual IContextsGenerator Create(IRTEvent irtEvent) =>
            Registry.Resolve(irtEvent.GetType());
    }
}
