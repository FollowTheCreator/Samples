using System;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Resources;
using Kernel.SharedDomain.Security;
using Kernel.Utilities.ValueObjects;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain
{
    public static class Operations
    {
        public static Type OperationsResourceType = typeof(FunctionOperationResources);

        public static class GenericDataTransfer
        {
            public const string ViewRaveJobsLogs = "f521a811-585f-473f-a3c2-d8b7e1432890";
            public const string ViewVeevaJobsLogs = "6ae13cb0-9640-424e-bd58-308f32a9d9e3";
            public const string ViewApacheHealthJobsLogs = "a4be2a1f-f036-4097-8aa5-df34d525a76f";

            public static StringResourceDescriptor DataTransferOperationGroup = new StringResourceDescriptor(OperationsResourceType, FunctionOperationResources.ResourceNames.GenericEdcDataTransferOperationGroup);

            public static SecuredOperation ViewRaveJobsLogsOperation =
                new SecuredOperation(
                    id: new Guid(ViewRaveJobsLogs),
                    operationGroup: DataTransferOperationGroup,
                    name: new StringResourceDescriptor(OperationsResourceType, FunctionOperationResources.ResourceNames.ViewRaveJobsLogs_Name),
                    description: new StringResourceDescriptor(OperationsResourceType, FunctionOperationResources.ResourceNames.ViewRaveJobsLogs_Description),
                    isUnblindedOperation: false);

            public static SecuredOperation ViewVeevaJobsLogsOperation =
                new SecuredOperation(
                    id: new Guid(ViewVeevaJobsLogs),
                    operationGroup: DataTransferOperationGroup,
                    name: new StringResourceDescriptor(OperationsResourceType, FunctionOperationResources.ResourceNames.ViewVeevaJobsLogs_Name),
                    description: new StringResourceDescriptor(OperationsResourceType, FunctionOperationResources.ResourceNames.ViewVeevaJobsLogs_Description),
                    isUnblindedOperation: false);

            public static SecuredOperation ViewApacheHealthJobsLogsOperation =
                new SecuredOperation(
                    id: new Guid(ViewApacheHealthJobsLogs),
                    operationGroup: DataTransferOperationGroup,
                    name: new StringResourceDescriptor(OperationsResourceType, FunctionOperationResources.ResourceNames.ViewApacheHealthJobsLogs_Name),
                    description: new StringResourceDescriptor(OperationsResourceType, FunctionOperationResources.ResourceNames.ViewApacheHealthJobsLogs_Description),
                    isUnblindedOperation: false);
        }
    }
}
