using System;
using Frameworks.Notifications;
using IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices;
using IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices.Models;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Errors;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Models;
using Kernel.Utilities.ValueObjects;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications
{
    public static class NotificationDefinitions
    {
        public const string OperationsGroup = "Generic Data Transfer";

        private static readonly string[] OperationsAllowedToUseEmailNotifications =
        [
            IRT.Domain.Operations.StudyAdministration.ViewEmailNotifications,
            IRT.Domain.Operations.StudyAdministration.ViewNotifications
        ];

        #region Error notifications
        public static NotificationDefinition RaveErrorNotification = new NotificationDefinition(new Guid("297F8097-A435-4198-8AC2-4769050D8BA9"))
        {
            NameResourceDescriptor = new StringResourceDescriptor(typeof(Resources.NotificationDefinitions),
            Resources.NotificationDefinitions.ResourceNames.RaveErrorForm),
            OperationsGroup = OperationsGroup,

            AllowedOperations = OperationsAllowedToUseEmailNotifications,

            // if a blinded user is to be subsrcibed to this, and it is confirmed no unblinding data exists in any of the forms, this can be updated in the study specific module to be false
            IsUnblinded = true,

            IsSiteSpecific = true,
            SendToLinkedAndAssociatedUsers = true,
            AllowSubscription = true,
            IsUsed = true,

            DataServiceType = typeof(ClientDataTransferErrorNotificationDataService),
            ViewModelType = typeof(ClientDataTransferErrorNotificationViewModel),

            TitleTemplateName = "IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.Templates.Errors.ClientErrorNotification.Title",
            BodyTemplateName = "IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.Templates.Errors.ClientErrorNotification",
        };

        public static NotificationDefinition VeevaErrorNotification = new NotificationDefinition(new Guid("297F8097-A435-4198-8AC2-4769050D8DE0"))
        {
            NameResourceDescriptor = new StringResourceDescriptor(typeof(Resources.NotificationDefinitions),
            Resources.NotificationDefinitions.ResourceNames.VeevaErrorForm),
            OperationsGroup = OperationsGroup,

            AllowedOperations = OperationsAllowedToUseEmailNotifications,

            // if a blinded user is to be subsrcibed to this, and it is confirmed no unblinding data exists in any of the forms, this can be updated in the study specific module to be false
            IsUnblinded = true,

            IsSiteSpecific = true,
            SendToLinkedAndAssociatedUsers = true,
            AllowSubscription = true,
            IsUsed = true,

            DataServiceType = typeof(ClientDataTransferErrorNotificationDataService),
            ViewModelType = typeof(ClientDataTransferErrorNotificationViewModel),

            TitleTemplateName = "IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.Templates.Errors.ClientErrorNotification.Title",
            BodyTemplateName = "IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.Templates.Errors.ClientErrorNotification",
        };

        public static NotificationDefinition RaveErrorsListNotification = new NotificationDefinition(new Guid("297F8097-A435-4198-8AC2-AC49E51CF582"))
        {
            NameResourceDescriptor = new StringResourceDescriptor(typeof(Resources.NotificationDefinitions),
            Resources.NotificationDefinitions.ResourceNames.RaveErrorFormsList),
            OperationsGroup = OperationsGroup,

            AllowedOperations = OperationsAllowedToUseEmailNotifications,

            IsSiteSpecific = false,
            SendToLinkedAndAssociatedUsers = false,
            AllowSubscription = true,
            IsUnblinded = false,
            IsUsed = true,

            DataServiceType = typeof(ClientDataTransferErrorSummaryNotificationDataService),
            ViewModelType = typeof(ClientDataTransferErrorSummaryNotificationViewModel),

            TitleTemplateName = "IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.Templates.Errors.ClientErrorSummaryNotification.Title",
            BodyTemplateName = "IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.Templates.Errors.ClientErrorSummaryNotification",
        };

        public static NotificationDefinition VeevaErrorsListNotification = new NotificationDefinition(new Guid("297F8097-A435-4198-8AC2-C9920C6333DD"))
        {
            NameResourceDescriptor = new StringResourceDescriptor(typeof(Resources.NotificationDefinitions),
            Resources.NotificationDefinitions.ResourceNames.VeevaErrorFormsList),
            OperationsGroup = OperationsGroup,

            AllowedOperations = OperationsAllowedToUseEmailNotifications,

            IsSiteSpecific = false,
            SendToLinkedAndAssociatedUsers = false,
            AllowSubscription = true,
            IsUnblinded = false,
            IsUsed = true,

            DataServiceType = typeof(ClientDataTransferErrorSummaryNotificationDataService),
            ViewModelType = typeof(ClientDataTransferErrorSummaryNotificationViewModel),

            TitleTemplateName = "IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.Templates.Errors.ClientErrorSummaryNotification.Title",
            BodyTemplateName = "IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.Templates.Errors.ClientErrorSummaryNotification",
        };
        #endregion

        #region Rave

        public static NotificationDefinition RaveClientEdcReceivedNotification = new NotificationDefinition(new Guid("297F8097-A435-4198-8AC2-472DF61D2015"))
        {
            NameResourceDescriptor = new StringResourceDescriptor(typeof(Resources.NotificationDefinitions),
            Resources.NotificationDefinitions.ResourceNames.RaveClientEdcReceivedData),
            OperationsGroup = OperationsGroup,

            IsUnblinded = true,

            DataServiceType = typeof(ReceivedNotificationDataService),
            ViewModelType = typeof(ReceivedNotificationViewModel),

            TitleTemplateName = "IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.Templates.ReceivedNotification.Title",
            BodyTemplateName = "IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.Templates.ReceivedNotification",
        };

        #endregion Rave
    }
}