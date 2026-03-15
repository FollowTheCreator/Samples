using Frameworks.ExtendedProperties.Dto;
using IRT.Modules.DataTransfer.Generic.Domain.Providers.ShouldGenerateNotificationProviders.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations
{
    public class NotificationGenerationService : INotificationGenerationService
    {
        private readonly IShouldGenerateNotificationPolicyRepository shouldGenerateNotificationProvider;

        public NotificationGenerationService(IShouldGenerateNotificationPolicyRepository shouldGenerateNotificationProvider)
        {
            this.shouldGenerateNotificationProvider = shouldGenerateNotificationProvider;
        }

        public virtual bool ShouldGenerateNotification(
            NotificationGenerationContext generationContext,
            ExtendedPropertyEntityDto<NotificationGenerationSettings> settings)
        {
            // Check if a provider is available for generating the notification
            if (!shouldGenerateNotificationProvider.TryGetServiceInstance(settings.Value.ShouldGenerateFormProviderId, out var providerInstance))
            {
                // If no provider is found, assume notifications should not be generated
                return false;
            }

            // Use the provider to determine if the notification should be generated
            return providerInstance.ShouldGenerateNotification(generationContext);
        }
    }
}
