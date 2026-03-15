using System;
using IRT.Modules.DataTransfer.Generic.Domain.Resources;
using Kernel.SharedDomain.Security;
using Kernel.Utilities.ValueObjects;
using ResourceKeys = IRT.Modules.DataTransfer.Generic.Domain.Resources.FunctionOperationResources.ResourceNames;

namespace IRT.Modules.DataTransfer.Generic.Domain
{
    public static class Operations
    {
        public static Type OperationsResourceType = typeof(FunctionOperationResources);

        public static class GenericDataTransfer
        {
            public const string ViewGenericNotifications = "297F8097-A435-4198-8AC2-0F07FC92E302";
            public const string ViewClientNotifications = "F112E26C-7958-4534-A87D-FBA40BD78B9A";
            public const string ViewManageGenericNotificationDefinitions = "a4b7d70f-ccc5-4e6c-94da-91166ceea803";
            public const string ViewManageNotificationsGeneration = "25f6c1c3-9999-476d-882d-d56957f75695";
            public const string ManageDynamicResources = "6b646495-4859-44af-88ed-a19e542511d3";
            public const string ManageDynamicSettings = "f0454389-ecb3-44a6-99ba-a7865f923f5d";

            public static StringResourceDescriptor DataTransferOperationGroup = new StringResourceDescriptor(OperationsResourceType, ResourceKeys.GenericDataTransferOperationGroup);

            public static SecuredOperation ViewGenericNotificationsOperation = new SecuredOperation(
                id: new Guid(ViewGenericNotifications),
                operationGroup: DataTransferOperationGroup,
                name: new StringResourceDescriptor(OperationsResourceType, ResourceKeys.ViewGenericNotifications),
                isUnblindedOperation: false);

            public static SecuredOperation ViewGenericNotificationsDefinitionsOperation = new SecuredOperation(
                id: new Guid(ViewManageGenericNotificationDefinitions),
                operationGroup: DataTransferOperationGroup,
                name: new StringResourceDescriptor(OperationsResourceType, ResourceKeys.ViewGenericNotificationsDefinitions),
                isUnblindedOperation: false);

            public static SecuredOperation ViewClientNotificationsOperation = new SecuredOperation(
                id: new Guid(ViewClientNotifications),
                operationGroup: DataTransferOperationGroup,
                name: new StringResourceDescriptor(OperationsResourceType, ResourceKeys.ViewClientNotifications),
                isUnblindedOperation: false);

            public static SecuredOperation ManageDynamicResourcesOperation = new SecuredOperation(
                id: new Guid(ManageDynamicResources),
                operationGroup: DataTransferOperationGroup,
                name: new StringResourceDescriptor(OperationsResourceType, ResourceKeys.ManageDynamicResources),
                isUnblindedOperation: false);

            public static SecuredOperation ManageDynamicSettingsOperation = new SecuredOperation(
               id: new Guid(ManageDynamicSettings),
               operationGroup: DataTransferOperationGroup,
               name: new StringResourceDescriptor(OperationsResourceType, ResourceKeys.ManageDynamicSettings),
               isUnblindedOperation: false);

            #region Manage Notification Generation

            public static SecuredOperation ViewManageNotificationsGenerationOperation = new SecuredOperation(
                    id: new Guid(ViewManageNotificationsGeneration),
                    operationGroup: DataTransferOperationGroup,
                    name: new StringResourceDescriptor(OperationsResourceType, ResourceKeys.ViewManageNotificationsGeneration));

            public static Guid[] NotificationGenerationManagementDefaultOperationIds = new[]
            {
                ViewManageNotificationsGenerationOperation.Id
            };

            public static Guid NotificationGenerationManagementDefaultOperationId = ViewManageNotificationsGenerationOperation.Id;

            #endregion
        }
    }
}
