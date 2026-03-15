using System;
using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations
{
    public abstract class BaseContextsGenerator<TEvent> : IContextsGenerator
        where TEvent : IRTEvent
    {
        public virtual BaseGenerationContext GetGenerationContext(IRTEvent irtEvent)
        {
            ValidateEventType(irtEvent);

            var typedEvent = (TEvent)irtEvent;

            return GetContextDetails(typedEvent);
        }

        protected virtual void ValidateEventType(IRTEvent irtEvent)
        {
            if (irtEvent is null)
            {
                throw new ArgumentNullException(nameof(irtEvent));
            }

            var providedType = irtEvent.GetType();

            if (typeof(TEvent).IsAssignableFrom(providedType))
            {
                return;
            }
        }

        protected abstract BaseGenerationContext GetContextDetails(TEvent irtEvent);

        protected virtual Guid ExtractUserId(TEvent irtEvent) =>
            Guid.Parse(irtEvent.CommandMetadata.UserId);
    }
}