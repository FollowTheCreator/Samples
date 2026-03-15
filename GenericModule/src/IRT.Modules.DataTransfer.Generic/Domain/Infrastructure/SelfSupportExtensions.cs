using System;
using System.Linq;
using IRT.Domain.ViewsSql.SelfSupportWorkflows;
using IRT.Domain.ViewsSql.SubjectOperationLog;
using IRT.Modules.DrugManagement.Contracts.ValueObjects.Drug;
using IRT.Modules.DrugManagement.Domain.Infrastructure.ModuleCaptureData;

namespace IRT.Modules.DataTransfer.Generic.Domain.Infrastructure
{
    public static class SelfSupportExtensions
    {
        public static T GetOldValue<T>(this SelfSupportModificationRequestSqlView request, SubjectOperationLogChangeKind changeKind)
        {
            return request.GetValue<T>(changeKind, x => x.OldValue);
        }

        public static T GetNewValue<T>(this SelfSupportModificationRequestSqlView request, SubjectOperationLogChangeKind changeKind)
        {
            return request.GetValue<T>(changeKind, x => x.NewValue);
        }

        public static T GetValue<T>(
            this SelfSupportModificationRequestSqlView request,
            SubjectOperationLogChangeKind changeKind,
            Func<SelfSupportModificationRequestDetailSqlView, string> valueSelector)
        {
            var rawValue = request?.Details
                ?.Where(x => x.ChangeKind == changeKind)
                .Select(valueSelector)
                .SingleOrDefault();

            if (rawValue == null)
            {
                return default(T);
            }

            var metadata = SubjectOperationLogChangeKindMetadata.Cache[changeKind];
            if (metadata.Deserializer(rawValue) is T value)
            {
                return value;
            }

            return default(T);
        }

        public static bool HasAnyBeenChanged(
           this SelfSupportModificationRequestSqlView request,
           params SubjectOperationLogChangeKind[] changeKinds)
        {
            if (request?.Details == null || changeKinds.Length == 0)
            {
                return false;
            }

            var wasChanged = request.Details
                .Any(x => changeKinds.Contains(x.ChangeKind));

            return wasChanged;
        }

        public static bool HasBirthYearChanged(this SelfSupportModificationRequestSqlView request)
        {
            var oldDate = request.GetOldValue<DateTime>(SubjectOperationLogChangeKind.DateOfBirth);
            var newDate = request.GetNewValue<DateTime>(SubjectOperationLogChangeKind.DateOfBirth);

            return oldDate.Year != newDate.Year;
        }

        public static SelfSupportModificationRequestDetailSqlView GetLabeledDrugAssignmentsDetail(
            this SelfSupportModificationRequestSqlView request)
        {
            return request.GetSelfSupportModificationRequestDetail(DrugManagementSubjectOperationLogChangeKind.LabeledDrugAssignments);
        }

        public static SelfSupportModificationRequestDetailSqlView GetLabeledDrugReplacementsDetail(
            this SelfSupportModificationRequestSqlView request)
        {
            return request.GetSelfSupportModificationRequestDetail(DrugManagementSubjectOperationLogChangeKind.LabeledDrugReplacements);
        }

        public static SelfSupportModificationRequestDetailSqlView GetSelfSupportModificationRequestDetail(
            this SelfSupportModificationRequestSqlView request,
            SubjectOperationLogChangeKind changeKind)
        {
            return request?.Details?.SingleOrDefault(x => x.ChangeKind == changeKind);
        }

        public static LabeledDrugWithAssignmentDate[] GetOldLabeledDrugAssignments(
           this SelfSupportModificationRequestDetailSqlView requestDetail)
        {
            if (requestDetail == null)
            {
                return null;
            }

            var metadata = SubjectOperationLogChangeKindMetadata.Cache[DrugManagementSubjectOperationLogChangeKind.LabeledDrugAssignments];

            var oldValue = (LabeledDrugWithAssignmentDate[])metadata.Deserializer(requestDetail.OldValue);

            return oldValue;
        }

        public static LabeledDrugReplacement<LabeledDrugWithAssignmentDate>[] GetOldLabeledDrugReplacements(
            this SelfSupportModificationRequestDetailSqlView requestDetail)
        {
            if (requestDetail == null)
            {
                return null;
            }

            var metadata = SubjectOperationLogChangeKindMetadata.Cache[DrugManagementSubjectOperationLogChangeKind.LabeledDrugReplacements];

            var oldValue = (LabeledDrugReplacement<LabeledDrugWithAssignmentDate>[])metadata.Deserializer(requestDetail.OldValue);

            return oldValue;
        }
    }
}
