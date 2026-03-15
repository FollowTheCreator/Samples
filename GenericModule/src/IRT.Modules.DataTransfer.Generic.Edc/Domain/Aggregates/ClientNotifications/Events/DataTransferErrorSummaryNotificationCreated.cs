using System;
using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.Errors;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Events
{
    public abstract class DataTransferErrorSummaryNotificationCreated : IRTEvent
    {
        public Guid NotificationId { get; set; }

        public Guid NotificationDefinitionId { get; set; }

        public ErrorListData[] FailedNotifcations { get; set; }
    }
}
