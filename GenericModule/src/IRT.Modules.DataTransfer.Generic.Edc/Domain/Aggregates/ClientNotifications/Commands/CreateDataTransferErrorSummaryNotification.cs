using System;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.Errors;
using Kernel.DDD.Domain.Commands;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands
{
    public abstract class CreateDataTransferErrorSummaryNotification : Command
    {
        public Guid NotificationId { get; set; }

        public Guid NotificationDefinitionId { get; set; }

        public ErrorListData[] FailedNotifications { get; set; }
    }
}
