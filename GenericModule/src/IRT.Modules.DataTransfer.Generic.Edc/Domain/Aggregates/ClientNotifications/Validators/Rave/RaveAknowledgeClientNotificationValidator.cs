using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands.Rave;
using Kernel.DDD.Domain.Validators;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Validators.Rave
{
    public class RaveAknowledgeClientNotificationValidator : CommandValidatorBase<RaveAknowledgeClientNotification>
    {
        protected override void Validate(RaveAknowledgeClientNotification message)
        {
        }
    }
}
