using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Commands;
using Kernel.DDD.Domain.Validators;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.ClientNotifications.Validators
{
    public class ReceiveClientNotificationValidator : CommandValidatorBase<ReceiveClientNotification>
    {
        protected override void Validate(ReceiveClientNotification message)
        {
        }
    }
}
