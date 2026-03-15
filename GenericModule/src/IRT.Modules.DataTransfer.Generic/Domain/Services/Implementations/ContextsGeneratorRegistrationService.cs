using System;
using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using Unity;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations
{
    public class ContextsGeneratorRegistrationService : IContextsGeneratorRegistrationService
    {
        private readonly IUnityContainer unityContainer;

        public ContextsGeneratorRegistrationService(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        public void Register<TEvent, TInstance>() where TEvent : IRTEvent where TInstance : IContextsGenerator
        {
            var registrationName = typeof(TEvent).FullName;
            unityContainer.RegisterType<IContextsGenerator, TInstance>(registrationName);
        }

        public IContextsGenerator Resolve(Type type) => unityContainer.Resolve<IContextsGenerator>(type.FullName);
    }
}
