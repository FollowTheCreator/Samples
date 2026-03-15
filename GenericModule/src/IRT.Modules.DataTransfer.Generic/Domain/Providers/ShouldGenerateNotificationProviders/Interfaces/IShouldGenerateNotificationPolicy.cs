using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts;

namespace IRT.Modules.DataTransfer.Generic.Domain.Providers.ShouldGenerateNotificationProviders.Interfaces
{
    public interface IShouldGenerateNotificationPolicy
    {
        public bool ShouldGenerateNotification(NotificationGenerationContext context);
    }
}