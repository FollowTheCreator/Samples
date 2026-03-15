using Frameworks.ExtendedProperties.Dto;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces
{
    public interface INotificationGenerationService
    {
        public bool ShouldGenerateNotification(NotificationGenerationContext generationContext, ExtendedPropertyEntityDto<NotificationGenerationSettings> notificationGenerationSettings);
    }
}
