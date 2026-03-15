using System;
using IRT.Domain;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces
{
    public interface IContextsGeneratorRegistrationService
    {
        void Register<TEvent, TInstance>()
            where TEvent : IRTEvent
            where TInstance : IContextsGenerator;

        IContextsGenerator Resolve(Type type);
    }
}