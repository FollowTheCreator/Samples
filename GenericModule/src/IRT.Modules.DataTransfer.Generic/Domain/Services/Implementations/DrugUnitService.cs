using System.Collections.Generic;
using System.Linq;
using IRT.Domain.ViewsSql.SelfSupportWorkflows;
using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;
using IRT.Modules.DrugManagement.Contracts.ValueObjects.Drug;
using IRT.Modules.DrugManagement.Domain.Infrastructure.ModuleCaptureData;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations
{
    public static class DrugUnitService
    {
        public static IEnumerable<string> BackOutLabeledDrugAssignments(SelfSupportModificationRequestSqlView request)
        {
            var oldDrugUnits = request.GetOldValue<LabeledDrugWithAssignmentDate[]>(DrugManagementSubjectOperationLogChangeKind.LabeledDrugAssignments);

            if (oldDrugUnits.IsNullOrEmpty())
            {
                return Enumerable.Empty<string>();
            }

            return oldDrugUnits.Select(x => x.DrugUnitId);
        }

        public static IEnumerable<string> BackOutLabeledDrugReplacements(SelfSupportModificationRequestSqlView request)
        {
            var oldDrugUnits = request.GetOldValue<LabeledDrugReplacement<LabeledDrugWithAssignmentDate>[]>(DrugManagementSubjectOperationLogChangeKind.LabeledDrugReplacements);

            if (oldDrugUnits.IsNullOrEmpty())
            {
                return Enumerable.Empty<string>();
            }

            return oldDrugUnits.Select(x => x.AssignedDrug.DrugUnitId);
        }

        public static void ProcessLabeledAssignments(
            SelfSupportModificationRequestSqlView request,
            List<string> assignedDrugIds = null,
            List<string> unassignedDrugIds = null)
        {
            if (assignedDrugIds is null
                && unassignedDrugIds is null)
            {
                return;
            }

            var oldDrugUnits = request.GetOldValue<LabeledDrugWithAssignmentDate[]>(DrugManagementSubjectOperationLogChangeKind.LabeledDrugAssignments);
            var newDrugUnits = request.GetNewValue<LabeledDrugWithAssignmentDate[]>(DrugManagementSubjectOperationLogChangeKind.LabeledDrugAssignments);

            if (oldDrugUnits.IsNullOrEmpty()
                && newDrugUnits.IsNullOrEmpty())
            {
                return;
            }

            assignedDrugIds?.AddRange(GetAddedDrugIds(oldDrugUnits, newDrugUnits));
            unassignedDrugIds?.AddRange(GetRemovedDrugIds(oldDrugUnits, newDrugUnits));
        }

        public static void ProcessLabeledReplacements(
            SelfSupportModificationRequestSqlView request,
            List<string> assignedDrugIds = null,
            List<string> unassignedDrugIds = null)
        {
            if (assignedDrugIds is null
                && unassignedDrugIds is null)
            {
                return;
            }

            var oldDrugReplacements = request.GetOldValue<LabeledDrugReplacement<LabeledDrugWithAssignmentDate>[]>(DrugManagementSubjectOperationLogChangeKind.LabeledDrugReplacements);
            var newDrugReplacements = request.GetNewValue<LabeledDrugReplacement<LabeledDrugWithAssignmentDate>[]>(DrugManagementSubjectOperationLogChangeKind.LabeledDrugReplacements);

            if (oldDrugReplacements.IsNullOrEmpty()
                && newDrugReplacements.IsNullOrEmpty())
            {
                return;
            }

            assignedDrugIds?.AddRange(GetAddedDrugIds(oldDrugReplacements, newDrugReplacements));
            unassignedDrugIds?.AddRange(GetRemovedDrugIds(oldDrugReplacements, newDrugReplacements));
        }

        public static List<string> GetAddedDrugIds(
            IEnumerable<LabeledDrugWithAssignmentDate> oldDrugUnits,
            IEnumerable<LabeledDrugWithAssignmentDate> newDrugUnits)
        {
            var addedDrugs = newDrugUnits
                .Safe()
                .Where(x => oldDrugUnits
                    .Safe()
                    .All(y => y.DrugUnitId != x.DrugUnitId))
                .ToList();

            return addedDrugs
                .Select(x => x.DrugUnitId)
                .ToList();
        }

        public static List<string> GetRemovedDrugIds(
            IEnumerable<LabeledDrugWithAssignmentDate> oldDrugUnits,
            IEnumerable<LabeledDrugWithAssignmentDate> newDrugUnits)
        {
            return oldDrugUnits
                .Safe()
                .Select(x => x.DrugUnitId)
                .Except(newDrugUnits
                    .Safe()
                    .Select(x => x.DrugUnitId))
                .ToList();
        }

        public static List<string> GetAddedDrugIds(
            IEnumerable<LabeledDrugReplacement<LabeledDrugWithAssignmentDate>> oldDrugReplacements,
            IEnumerable<LabeledDrugReplacement<LabeledDrugWithAssignmentDate>> newDrugReplacements)
        {
            var addedDrugs = newDrugReplacements
                .Safe()
                .Where(x => oldDrugReplacements.Safe().All(y => y.AssignedDrug.DrugUnitId != x.AssignedDrug.DrugUnitId))
                .ToList();

            return addedDrugs
                .Select(x => x.AssignedDrug.DrugUnitId)
                .ToList();
        }

        public static List<string> GetRemovedDrugIds(
            IEnumerable<LabeledDrugReplacement<LabeledDrugWithAssignmentDate>> oldDrugReplacements,
            IEnumerable<LabeledDrugReplacement<LabeledDrugWithAssignmentDate>> newDrugReplacements)
        {
            var removedDrugs = oldDrugReplacements
                .Safe()
                .Where(x => newDrugReplacements.Safe().All(y => y.AssignedDrug.DrugUnitId != x.AssignedDrug.DrugUnitId))
                .ToList();

            return removedDrugs
                .Select(x => x.AssignedDrug.DrugUnitId)
                .ToList();
        }
    }
}
