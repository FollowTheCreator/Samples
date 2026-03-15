using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands;
using Kernel.DDD.Domain.Validators;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Validators
{
    public class AknowledgeClientNotificationValidator : CommandValidatorBase<AcknowledgeClientNotification>
    {
        protected override void Validate(AcknowledgeClientNotification message)
        {
        }
    }
}
