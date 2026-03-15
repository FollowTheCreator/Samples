using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain.ViewsSql.SelfSupportWorkflows;
using IRT.Domain.ViewsSql.Study;
using IRT.Domain.ViewsSql.Subject.SubjectVisitDrug;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;

namespace IRT.Modules.DataTransfer.Generic.Edc.Helpers.RepeatKey
{
    public class SerializableItemGroupRepeatKeyHelper
    {
        private readonly IQueryable<GenericItemGroupRepeatKeySqlView> genericItemGroupRepeatKeysQuery;
        private readonly IQueryable<SubjectVisitLabeledDrugSqlView> subjectVisitsLabeledDrugsQuery;

        private readonly GenericDataTransferStudySettings genericStudySettings;

        public SerializableItemGroupRepeatKeyHelper(
            IQueryable<GenericItemGroupRepeatKeySqlView> genericItemGroupRepeatKeysQuery,
            IQueryable<SubjectVisitLabeledDrugSqlView> subjectVisitsLabeledDrugQuery,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider)
        {
            this.genericItemGroupRepeatKeysQuery = genericItemGroupRepeatKeysQuery;
            subjectVisitsLabeledDrugsQuery = subjectVisitsLabeledDrugQuery;
            genericStudySettings = extendedPropertiesValueProvider.GetValue<StudySqlView, GenericDataTransferStudySettings>(null);
        }

        public List<SerializableDrugUnit> GetSerializableDrugUnits(NotificationSqlView notification)
        {
            var drugs = subjectVisitsLabeledDrugsQuery
                .Where(x => x.SubjectVisitId == notification.SubjectVisitId)
                .GroupJoin(
                    genericItemGroupRepeatKeysQuery
                        .Where(x => x.NotificationDefinitionId == notification.NotificationDefinitionId),
                    x => x.AssignedDrugUnitId,
                    y => y.DrugUnitId,
                    (x, y) => new
                    {
                        SubjectVisitLabeledDrug = x,
                        DrugItemGroupRepeatKeys = y
                    })
                .SelectMany(
                    x => x.DrugItemGroupRepeatKeys.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        SubjectVisitLabeledDrug = x.SubjectVisitLabeledDrug,
                        AssignedRepeatKey = y.RepeatKey,
                        AssignedTransactionType = y.TransactionType
                    })
                .GroupJoin(
                    genericItemGroupRepeatKeysQuery
                        .Where(x => x.NotificationDefinitionId == notification.NotificationDefinitionId),
                    x => x.SubjectVisitLabeledDrug.ReplacedDrugUnitId,
                    y => y.DrugUnitId,
                    (x, y) => new
                    {
                        Drug = x,
                        DrugItemGroupRepeatKeys = y
                    })
                .SelectMany(
                    x => x.DrugItemGroupRepeatKeys.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        x.Drug.SubjectVisitLabeledDrug,
                        x.Drug.AssignedRepeatKey,
                        x.Drug.AssignedTransactionType,
                        ReplacedRepeatKey = y.RepeatKey,
                        ReplacedTransactionType = y.TransactionType
                    })
                .OrderBy(x => x.AssignedRepeatKey)
                .Select(x => new SerializableDrugUnit
                {
                    SubjectVisitLabeledDrug = x.SubjectVisitLabeledDrug,
                    SubjectVisitLabeledDrugToRestore = null,
                    AssignedItemGroupRepeatKey = new SerializableRepeatKey
                    {
                        RepeatKey = x.AssignedRepeatKey.ToString(),
                        TransactionType = x.AssignedTransactionType
                    },
                    ReplacedItemGroupRepeatKey = x.ReplacedRepeatKey != null
                        ? new SerializableRepeatKey
                        {
                            RepeatKey = x.ReplacedRepeatKey.ToString(),
                            TransactionType = x.ReplacedTransactionType
                        }
                        : null
                })
                .ToList();

            return drugs;
        }

        public List<SerializableDrugUnit> GetSelfSupportSerializableDrugUnits(
            SelfSupportModificationRequestSqlView request,
            GenericEdcNotificationDefinitionSettings genericNotificationDefinitionSettings,
            NotificationSqlView notification)
        {
            var assignedDrugIds = new List<string>();
            var unassignedDrugIds = new List<string>();

            var addedReplacementDrugIds = new List<string>();
            var removedReplacementDrugIds = new List<string>();

            DrugUnitService.ProcessLabeledAssignments(request, assignedDrugIds, unassignedDrugIds);
            DrugUnitService.ProcessLabeledReplacements(request, addedReplacementDrugIds, removedReplacementDrugIds);

            if (assignedDrugIds.Count == 0
                && unassignedDrugIds.Count == 0
                && addedReplacementDrugIds.Count == 0
                && removedReplacementDrugIds.Count == 0)
            {
                return null;
            }

            var assignedRepeatKeys = genericItemGroupRepeatKeysQuery
                .Where(x => assignedDrugIds.Contains(x.DrugUnitId)
                    && x.NotificationDefinitionId == notification.NotificationDefinitionId)
                .Select(x => new
                {
                    x.DrugUnitId,
                    x.RepeatKey,
                    TransactionType = genericNotificationDefinitionSettings.ItemGroupRepeatKeyInsertTransactionType
                });

            var assignedDrugs = subjectVisitsLabeledDrugsQuery
                .Where(x => x.SubjectVisitId == notification.SubjectVisitId
                    && (assignedDrugIds.Contains(x.AssignedDrugUnitId)))
                .GroupJoin(
                    assignedRepeatKeys,
                    x => x.AssignedDrugUnitId,
                    y => y.DrugUnitId,
                    (x, y) => new
                    {
                        SubjectVisitLabeledDrug = x,
                        DrugItemGroupRepeatKeys = y
                    })
                .SelectMany(
                    x => x.DrugItemGroupRepeatKeys.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        SubjectVisitLabeledDrug = x.SubjectVisitLabeledDrug,
                        SubjectVisitLabeledDrugToRestore = (SubjectVisitLabeledDrugSqlView)null,
                        AssignedRepeatKey = y.RepeatKey,
                        AssignedTransactionType = y.TransactionType,
                        ReplacedRepeatKey = (int?)null,
                        ReplacedTransactionType = (string)null
                    })
                .ToList();

            var unassignedDrugs = genericItemGroupRepeatKeysQuery
                .Where(x => unassignedDrugIds.Contains(x.DrugUnitId)
                    && x.NotificationDefinitionId == notification.NotificationDefinitionId)
                .Select(x => new
                {
                    SubjectVisitLabeledDrug = (SubjectVisitLabeledDrugSqlView)null,
                    SubjectVisitLabeledDrugToRestore = (SubjectVisitLabeledDrugSqlView)null,
                    AssignedRepeatKey = x.RepeatKey,
                    AssignedTransactionType = genericNotificationDefinitionSettings.ItemGroupRepeatKeyRemoveTransactionType,
                    ReplacedRepeatKey = (int?)null,
                    ReplacedTransactionType = (string)null
                })
                .ToList();

            var addedReplacementDrugs = subjectVisitsLabeledDrugsQuery
                .Where(x => x.SubjectVisitId == notification.SubjectVisitId
                    && (addedReplacementDrugIds.Contains(x.AssignedDrugUnitId)))
                .GroupJoin(
                    genericItemGroupRepeatKeysQuery
                        .Where(x => x.NotificationDefinitionId == notification.NotificationDefinitionId),
                    x => x.AssignedDrugUnitId,
                    y => y.DrugUnitId,
                    (x, y) => new
                    {
                        SubjectVisitLabeledDrug = x,
                        RepeatKeys = y
                    })
                .SelectMany(
                    x => x.RepeatKeys.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        SubjectVisitLabeledDrug = x.SubjectVisitLabeledDrug,
                        SubjectVisitLabeledDrugToRestore = (SubjectVisitLabeledDrugSqlView)null,
                        AssignedRepeatKey = y.RepeatKey,
                        AssignedTransactionType = y.TransactionType
                    })
                .GroupJoin(
                    genericItemGroupRepeatKeysQuery
                        .Where(x => x.NotificationDefinitionId == notification.NotificationDefinitionId),
                    x => x.SubjectVisitLabeledDrug.ReplacedDrugUnitId,
                    y => y.DrugUnitId,
                    (x, y) => new
                    {
                        Drug = x,
                        RepeatKeys = y
                    })
                .SelectMany(
                    x => x.RepeatKeys.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        x.Drug.SubjectVisitLabeledDrug,
                        x.Drug.SubjectVisitLabeledDrugToRestore,
                        x.Drug.AssignedRepeatKey,
                        x.Drug.AssignedTransactionType,
                        ReplacedRepeatKey = y.RepeatKey,
                        ReplacedTransactionType = y.TransactionType
                    })
                .ToList();

            var removedReplacementRepeatKeys = genericItemGroupRepeatKeysQuery
                .Where(x => removedReplacementDrugIds.Contains(x.DrugUnitId)
                    && x.NotificationDefinitionId == notification.NotificationDefinitionId)
                .GroupJoin(
                    subjectVisitsLabeledDrugsQuery,
                    x => x.DrugUnitIdToRestore,
                    y => y.AssignedDrugUnitId,
                    (x, y) => new
                    {
                        ItemGroupRepeatKey = x,
                        SubjectVisitLabeledDrugToRestore = y
                    })
                .SelectMany(
                    x => x.SubjectVisitLabeledDrugToRestore.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        SubjectVisitLabeledDrugToRestore = y,
                        x.ItemGroupRepeatKey.DrugUnitId,
                        AssignedRepeatKey = x.ItemGroupRepeatKey.RepeatKey,
                        AssignedTransactionType = y != null
                            ? genericNotificationDefinitionSettings.ItemGroupRepeatKeyUpdateTransactionType
                            : genericNotificationDefinitionSettings.ItemGroupRepeatKeyRemoveTransactionType
                    })
                .GroupJoin(// contains RK information of the drug that has been replaced by the drug that is being removed
                    genericItemGroupRepeatKeysQuery,
                    x => x.SubjectVisitLabeledDrugToRestore.AssignedDrugUnitId,
                    y => y.DrugUnitId,
                    (x, y) => new
                    {
                        Drug = x,
                        RepeatKeys = y
                    })
                .SelectMany(
                    x => x.RepeatKeys.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        x.Drug.SubjectVisitLabeledDrugToRestore,
                        x.Drug.DrugUnitId,
                        x.Drug.AssignedRepeatKey,
                        x.Drug.AssignedTransactionType,
                        ReplacedRepeatKey = y.RepeatKey,
                        ReplacedTransactionType = y.TransactionType
                    })
                .ToList();

            var removedReplacementDrugs = removedReplacementRepeatKeys
                .Select(x => new
                {
                    SubjectVisitLabeledDrug = (SubjectVisitLabeledDrugSqlView)null,
                    SubjectVisitLabeledDrugToRestore = x.SubjectVisitLabeledDrugToRestore,
                    AssignedRepeatKey = x.AssignedRepeatKey,
                    AssignedTransactionType = x.AssignedTransactionType,
                    ReplacedRepeatKey = x.ReplacedRepeatKey,
                    ReplacedTransactionType = x.ReplacedTransactionType,
                })
                .ToList();

            var selfSupportDrugsToSerialize = assignedDrugs
                .Concat(unassignedDrugs)
                .Concat(addedReplacementDrugs)
                .Concat(removedReplacementDrugs)
                .OrderBy(x => x.AssignedRepeatKey)
                .Select(x => new SerializableDrugUnit
                {
                    SubjectVisitLabeledDrug = x.SubjectVisitLabeledDrug,
                    SubjectVisitLabeledDrugToRestore = x.SubjectVisitLabeledDrugToRestore,
                    AssignedItemGroupRepeatKey = new SerializableRepeatKey
                    {
                        RepeatKey = x.AssignedRepeatKey.ToString(),
                        TransactionType = x.AssignedTransactionType
                    },
                    ReplacedItemGroupRepeatKey = x.ReplacedRepeatKey != null
                        ? new SerializableRepeatKey
                        {
                            RepeatKey = x.ReplacedRepeatKey.ToString(),
                            TransactionType = x.ReplacedTransactionType
                        }
                        : null
                })
                .ToList();

            return selfSupportDrugsToSerialize;
        }

        public List<SerializableDrugUnit> GetBackoutSerializableDrugUnits(
            SelfSupportModificationRequestSqlView request,
            GenericEdcNotificationDefinitionSettings genericNotificationDefinitionSettings,
            NotificationSqlView notification)
        {
            var backoutReplacementDrugs = subjectVisitsLabeledDrugsQuery
                .Where(x => x.SubjectVisitId == notification.SubjectVisitId
                    && x.ReplacedDrugUnitId != null)
                .GroupJoin(
                    genericItemGroupRepeatKeysQuery
                        .Where(x => x.NotificationDefinitionId == notification.NotificationDefinitionId),
                    x => x.ReplacedDrugUnitId,
                    y => y.DrugUnitId,
                    (x, y) => new
                    {
                        Drug = x,
                        DrugItemGroupRepeatKeys = y
                    })
                .SelectMany(
                    x => x.DrugItemGroupRepeatKeys.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        x.Drug.AssignedDrugUnitId,
                        ReplacedItemGroupRepeatKey = y.RepeatKey != null
                            ? new SerializableRepeatKey
                            {
                                RepeatKey = y.RepeatKey.ToString(),
                                TransactionType = y.TransactionType
                            }
                            : null
                    })
                .ToList();

            var backoutRepeatKeys = genericItemGroupRepeatKeysQuery
                .Where(x => request.SubjectVisitId == x.SubjectVisitId
                    && x.NotificationDefinitionId == notification.NotificationDefinitionId)
                .GroupJoin(
                    genericItemGroupRepeatKeysQuery
                        .Where(x => x.NotificationDefinitionId == notification.NotificationDefinitionId),
                    x => x.DrugUnitIdToRestore,
                    x => x.DrugUnitId,
                    (x, y) => new
                    {
                        x.RepeatKey,
                        x.DrugUnitId,
                        x.IsSelfSupportDrugUnit,
                        ReplacedRepeatKey = y
                    })
                .SelectMany(
                    x => x.ReplacedRepeatKey.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        x.DrugUnitId,
                        x.RepeatKey,
                        x.IsSelfSupportDrugUnit,
                        ShouldBeSkipped = request.SubjectVisitId == y.SubjectVisitId // for the restoring repeat keys from a different visit do not send same repeat key multiple times but only once
                            && y.DrugUnitId != null,
                        DrugUnitIdToRestore = request.SubjectVisitId != y.SubjectVisitId // restore replaced drugs only from a different visit
                            ? y.DrugUnitId
                            : null
                    })
                .Where(x => !x.ShouldBeSkipped)
                .ToList();

            var subjectVisitLabeledDrugsToRestore = subjectVisitsLabeledDrugsQuery
                .GroupJoin(
                    genericItemGroupRepeatKeysQuery
                        .Where(x => x.NotificationDefinitionId == notification.NotificationDefinitionId),
                    x => x.AssignedDrugUnitId,
                    x => x.DrugUnitId,
                    (x, y) => new
                    {
                        SubjectVisitLabeledDrug = x,
                        RepeatKeys = y
                    })
                .SelectMany(
                    x => x.RepeatKeys.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        x.SubjectVisitLabeledDrug,
                        y.RepeatKey,
                        y.TransactionType
                    })
                .Where(x => backoutRepeatKeys
                    .Select(y => y.DrugUnitIdToRestore)
                    .Contains(x.SubjectVisitLabeledDrug.AssignedDrugUnitId))
                .ToList();

            var backoutDrugsToSerialize = backoutRepeatKeys
                .OrderBy(x => x.RepeatKey)
                .Select(x =>
                {
                    var subjectVisitLabeledDrugToRestore = subjectVisitLabeledDrugsToRestore
                        .FirstOrDefault(y => y.SubjectVisitLabeledDrug.AssignedDrugUnitId == x.DrugUnitIdToRestore);

                    var replacedDrug = backoutReplacementDrugs
                        .FirstOrDefault(y => y.AssignedDrugUnitId == x.DrugUnitId);

                    return new SerializableDrugUnit
                    {
                        SubjectVisitLabeledDrug = null,
                        SubjectVisitLabeledDrugToRestore = subjectVisitLabeledDrugToRestore?.SubjectVisitLabeledDrug,
                        AssignedItemGroupRepeatKey = new SerializableRepeatKey
                        {
                            RepeatKey = x.RepeatKey.ToString(),
                            TransactionType = subjectVisitLabeledDrugToRestore?.SubjectVisitLabeledDrug == null
                                ? genericNotificationDefinitionSettings.ItemGroupRepeatKeyRemoveTransactionType
                                : genericNotificationDefinitionSettings.ItemGroupRepeatKeyUpdateTransactionType
                        },
                        ReplacedItemGroupRepeatKey = replacedDrug?.ReplacedItemGroupRepeatKey?.RepeatKey != null
                            ? replacedDrug.ReplacedItemGroupRepeatKey
                            : null
                    };
                })
                .ToList();

            return backoutDrugsToSerialize;
        }
    }
}