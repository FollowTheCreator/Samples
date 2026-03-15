namespace IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts
{
    public class NotificationGenerationContext
    {
        public NotificationGenerationContext(
            BaseGenerationContext baseGenerationContext,
            string notificationEntityId)
        {
            if (string.IsNullOrWhiteSpace(notificationEntityId))
            {
                throw new System.ArgumentException($"'{nameof(notificationEntityId)}' cannot be null or whitespace.", nameof(notificationEntityId));
            }

            BaseGenerationContext = baseGenerationContext ?? throw new System.ArgumentNullException(nameof(baseGenerationContext));
            NotificationEntityId = notificationEntityId;
        }

        public BaseGenerationContext BaseGenerationContext { get; }

        public virtual string NotificationEntityId { get; }
    }
}