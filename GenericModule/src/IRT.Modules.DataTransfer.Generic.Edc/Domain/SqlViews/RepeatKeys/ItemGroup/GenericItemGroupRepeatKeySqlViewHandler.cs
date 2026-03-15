using Microsoft.EntityFrameworkCore;
using NLog;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain.Aggregates.SelfSupportWorkflowOrchestrator.ValueObjects;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Domain.Constants;
using IRT.Domain.Notifications;
using IRT.Domain.ViewsSql;
using IRT.Domain.ViewsSql.SelfSupportWorkflows;
using IRT.Domain.ViewsSql.Study;
using IRT.Domain.ViewsSql.Subject;
using IRT.Domain.ViewsSql.Subject.SubjectVisitDrug;
using IRT.Domain.ViewsSql.SubjectOperationLog;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.GenericDataGeneration.Events;
using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Edc.Helpers.RepeatKey;
using Kernel.DDD.Dispatching;
using Kernel.Globalization.Utilities;
using Kernel.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup
{
    [Priority(EventHandlerPriorityStages.ProjectedVisitsCalculation)]
    public class GenericItemGroupRepeatKeySqlViewHandler : SqlViewHandlerBase
    {
        private readonly IQueryable<SubjectVisitSqlView> subjectVisitsQuery;
        private readonly IQueryable<SubjectVisitCapturedDataSqlView> subjectVisitCapturedDataQuery;
        private readonly IQueryable<SubjectVisitLabeledDrugSqlView> subjectVisitLabeledDrugsQuery;

        private readonly GenericDataTransferStudySettings genericStudySettings;

        private readonly ItemGroupRepeatKeyHelper itemGroupRepeatKeyHelper;

        private readonly IEventGenerationService eventGenerationService;
        private readonly IFailReasonsService failReasonsService;

        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public GenericItemGroupRepeatKeySqlViewHandler(
            IQueryable<SubjectVisitSqlView> subjectVisitsQuery,
            IQueryable<SubjectVisitCapturedDataSqlView> subjectVisitCapturedDataQuery,
            IQueryable<SubjectVisitLabeledDrugSqlView> subjectVisitLabeledDrugsQuery,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            ItemGroupRepeatKeyHelper itemGroupRepeatKeyHelper,
            IEventGenerationService eventGenerationService,
            IFailReasonsService failReasonsService)
        {
            this.subjectVisitsQuery = subjectVisitsQuery;
            this.subjectVisitCapturedDataQuery = subjectVisitCapturedDataQuery;
            this.subjectVisitLabeledDrugsQuery = subjectVisitLabeledDrugsQuery;
            this.itemGroupRepeatKeyHelper = itemGroupRepeatKeyHelper;
            this.eventGenerationService = eventGenerationService;
            this.failReasonsService = failReasonsService;
            genericStudySettings = extendedPropertiesValueProvider.GetValue<StudySqlView, GenericDataTransferStudySettings>(null);
        }

        public void Handle(SubjectVisitPerformed subjectVisitPerformed) //TODO move every RK handling logic to a separate service and add the ability to create and connect/use custom handlers for each of the NotificationDefinitionIds (similar to custom triggers)
        {
            try
            {
                IEnumerable<GenericIntegrationDataEvent> generatedEvents = new List<GenericIntegrationDataEvent>();
                CultureHelper.ExecuteInCulture(() => generatedEvents = eventGenerationService.CreateGenericDataEvent(subjectVisitPerformed));

                var eventsInfo = generatedEvents
                    .Select(x => new
                    {
                        x.NotificationDefinitionID,
                        x.SubjectId,
                        x.SubjectVisitId,
                        VisitId = x.VisitId != null
                            ? x.VisitId
                            : x.VisitContext != null
                                ? x.VisitContext.VisitId
                                : null,
                        x.SiteId
                    })
                    .Distinct();

                var subjectVisitsCache = subjectVisitsQuery
                    .AsNoTracking()
                    .Where(x => eventsInfo
                        .Select(y => y.SubjectVisitId)
                        .Contains(x.Id))
                    .Select(x => new
                    {
                        x.Id,
                        x.VisitDate,
                        x.VisitId,
                        x.ParentSubjectVisitId,
                        ParentVisitId = x.ParentSubjectVisit.VisitId,
                        ScreenFailReasonsString = x.SubjectVisitCapturedData
                            .Where(y => y.FieldName == CapturedDataFields.ScreenFailReason)
                            .Select(y => y.Value)
                            .FirstOrDefault(),
                        Drugs = x.LabeledDrugs
                            .Select(y => new DrugInfo
                            {
                                AssignedDrugUnitId = y.AssignedDrugUnitId,
                                ReplacedDrugUnitId = y.ReplacedDrugUnitId,
                                DrugCode = y.AssignedDrugUnit.DrugDetails.DrugCode
                            })
                            .OrderBy(y => y.AssignedDrugUnitId)
                            .ToList()
                    })
                    .ToList();

                foreach (var eventInfo in eventsInfo)
                {
                    var notificationDefinition = NotificationDefinitionRegistry.GetDefinition(eventInfo.NotificationDefinitionID);

                    var notificationDefinitionSettings = ExtendedPropertiesValueProvider
                        .GetValue<NotificationDefinitionSqlView, GenericEdcNotificationDefinitionSettings>(notificationDefinition.Id.ToString());

                    if (!notificationDefinitionSettings.ItemGroupRepeatKeysEnabled)
                    {
                        continue;
                    }

                    var subjectVisitInfo = subjectVisitsCache
                        .Where(x => x.Id == eventInfo.SubjectVisitId
                            && (notificationDefinitionSettings.ItemGroupRepeatKeyCountSkippedVisits // exclude skipped visits
                                || x.VisitDate.HasValue)
                            && (notificationDefinitionSettings.ItemGroupRepeatKeysVisitsToExclude.Length == 0 // process excluded visits
                                || !notificationDefinitionSettings.ItemGroupRepeatKeysVisitsToExclude
                                    .Contains(x.VisitId))
                            && (notificationDefinitionSettings.ItemGroupRepeatKeysVisitsToInclude.Length == 0 // process included visits (include all if empty)
                                || notificationDefinitionSettings.ItemGroupRepeatKeysVisitsToInclude
                                    .Contains(x.VisitId)))
                        .Select(x => new
                        {
                            ParentSubjectVisitId = x.ParentSubjectVisitId,
                            ParentVisitId = x.ParentVisitId,
                            VisitDate = x.VisitDate,
                            Drugs = x.Drugs
                                .Where(y => !notificationDefinitionSettings.ItemGroupRepeatKeysDrugCodesToExclude // exclude drug codes
                                    .Contains(y.DrugCode))
                                .ToList(),
                            ScreenFailReasons = failReasonsService.GetFailReasons(x.ScreenFailReasonsString, true)
                        })
                        .FirstOrDefault();

                    if (subjectVisitInfo != null)
                    {
                        var repeatKeyFilter = new ItemGroupRepeatKeyFilterModel(notificationDefinitionSettings)
                        {
                            NotificationDefinitionId = notificationDefinition.Id,
                            Drugs = subjectVisitInfo.Drugs,
                            ScreenFailReasons = subjectVisitInfo.ScreenFailReasons,
                            SubjectId = eventInfo.SubjectId,
                            SubjectVisitId = eventInfo.SubjectVisitId,
                            ParentSubjectVisitId = subjectVisitInfo.ParentSubjectVisitId,
                            VisitId = eventInfo.VisitId,
                            ParentVisitId = subjectVisitInfo.ParentVisitId,
                            SiteId = eventInfo.SiteId,
                            SelfSupport = false
                        };

                        InsertRepeatKeys(repeatKeyFilter);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in the ItemGroup Repeat Key SubjectVisitPerformed handler");
            }
        }

        public void Handle(SelfSupportChangeRequestProcessed selfSupportChangeRequestProcessed)
        {
            try
            {
                if (selfSupportChangeRequestProcessed.Status != SelfSupportRequestStatus.Confirmed)
                {
                    return;
                }

                var request = Db.SelfSupportModificationRequests
                    .AsNoTracking()
                    .Include(x => x.Details)
                    .Single(x => x.RequestId == selfSupportChangeRequestProcessed.RequestId);

                if (request.DataChangeType == SubjectSelfSupportDataChangeType.BackOutTransaction)
                {
                    return;
                }

                IEnumerable<GenericIntegrationDataEvent> generatedEvents = new List<GenericIntegrationDataEvent>();
                CultureHelper.ExecuteInCulture(() => generatedEvents = eventGenerationService.CreateGenericDataEvent(selfSupportChangeRequestProcessed));

                var eventsInfo = generatedEvents
                    .Select(x => new
                    {
                        x.NotificationDefinitionID,
                        x.SubjectId,
                        x.SubjectVisitId,
                        VisitId = x.VisitId != null
                            ? x.VisitId
                            : x.VisitContext != null
                                ? x.VisitContext.VisitId
                                : null,
                        x.SiteId
                    })
                    .Distinct();

                var subjectVisitsCache = subjectVisitsQuery
                    .AsNoTracking()
                    .Where(x => eventsInfo
                        .Select(y => y.SubjectVisitId)
                        .Contains(x.Id))
                    .Select(x => new
                    {
                        x.Id,
                        x.VisitDate,
                        x.VisitId,
                        x.ParentSubjectVisitId,
                        ParentVisitId = x.ParentSubjectVisit.VisitId
                    })
                    .ToList();

                foreach (var eventInfo in eventsInfo)
                {
                    var notificationDefinition = NotificationDefinitionRegistry.GetDefinition(eventInfo.NotificationDefinitionID);

                    var notificationDefinitionSettings = ExtendedPropertiesValueProvider
                        .GetValue<NotificationDefinitionSqlView, GenericEdcNotificationDefinitionSettings>(notificationDefinition.Id.ToString());

                    if (!notificationDefinitionSettings.ItemGroupRepeatKeysEnabled)
                    {
                        continue;
                    }

                    var subjectVisitInfo = subjectVisitsCache
                        .Where(x => x.Id == eventInfo.SubjectVisitId
                            && (notificationDefinitionSettings.ItemGroupRepeatKeyCountSkippedVisits // exclude skipped visits
                                || x.VisitDate.HasValue)
                            && (notificationDefinitionSettings.ItemGroupRepeatKeysVisitsToExclude.Length == 0 // process excluded visits
                                || !notificationDefinitionSettings.ItemGroupRepeatKeysVisitsToExclude
                                    .Contains(x.VisitId))
                            && (notificationDefinitionSettings.ItemGroupRepeatKeysVisitsToInclude.Length == 0 // process included visits (include all if empty)
                                || notificationDefinitionSettings.ItemGroupRepeatKeysVisitsToInclude
                                    .Contains(x.VisitId)))
                        .Select(x => new
                        {
                            ParentSubjectVisitId = x.ParentSubjectVisitId,
                            ParentVisitId = x.ParentVisitId,
                            VisitDate = x.VisitDate
                        })
                        .FirstOrDefault();

                    if (subjectVisitInfo != null)
                    {
                        var repeatKeyFilter = new ItemGroupRepeatKeyFilterModel(notificationDefinitionSettings)
                        {
                            NotificationDefinitionId = notificationDefinition.Id,
                            SubjectId = eventInfo.SubjectId,
                            SubjectVisitId = eventInfo.SubjectVisitId,
                            ParentSubjectVisitId = subjectVisitInfo.ParentSubjectVisitId,
                            VisitId = eventInfo.VisitId,
                            ParentVisitId = subjectVisitInfo.ParentVisitId,
                            SiteId = eventInfo.SiteId,
                            SelfSupport = true
                        };

                        switch (request.DataChangeType)
                        {
                            case SubjectSelfSupportDataChangeType.UpdateVisitData:
                                if (request.HasAnyBeenChanged(SubjectOperationLogChangeKind.ScreenFailReason))
                                {
                                    HandleScreenFailReasonsUpdate(repeatKeyFilter);
                                }

                                break;

                            case SubjectSelfSupportDataChangeType.UpdateAssignedDrugs:
                                HandleUpdateAssignedDrugs(
                                    request,
                                    repeatKeyFilter,
                                    notificationDefinitionSettings);

                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in the ItemGroup Repeat Key SelfSupportChangeRequestProcessed handler");
            }
        }

        private void HandleScreenFailReasonsUpdate(ItemGroupRepeatKeyFilterModel repeatKeyFilter)
        {
            var screenFailReasons = subjectVisitCapturedDataQuery
                .Where(y => y.SubjectVisitId == repeatKeyFilter.SubjectVisitId
                    && y.FieldName == CapturedDataFields.ScreenFailReason)
                .Select(y => failReasonsService.GetFailReasons(y.Value, true))
                .FirstOrDefault();

            repeatKeyFilter.ScreenFailReasons = screenFailReasons;

            InsertRepeatKeys(repeatKeyFilter);
        }

        private void HandleUpdateAssignedDrugs(
            SelfSupportModificationRequestSqlView request,
            ItemGroupRepeatKeyFilterModel repeatKeyFilter,
            GenericEdcNotificationDefinitionSettings notificationDefinitionSettings)
        {
            var assignedDrugIds = new List<string>();

            DrugUnitService.ProcessLabeledAssignments(request, assignedDrugIds);
            DrugUnitService.ProcessLabeledReplacements(request, assignedDrugIds);

            var assignedDrugs = subjectVisitLabeledDrugsQuery
                .Where(x => assignedDrugIds.Contains(x.AssignedDrugUnitId))
                .Select(x => new DrugInfo
                {
                    AssignedDrugUnitId = x.AssignedDrugUnitId,
                    ReplacedDrugUnitId = x.ReplacedDrugUnitId,
                    DrugCode = x.AssignedDrugUnit.DrugDetails.DrugCode
                })
                .ToList();

            var drugsToInsert = assignedDrugs
                .Where(y => !notificationDefinitionSettings.ItemGroupRepeatKeysDrugCodesToExclude // exclude drug codes
                    .Contains(y.DrugCode))
                .OrderBy(y => y.AssignedDrugUnitId)
                .ToList();

            repeatKeyFilter.Drugs = drugsToInsert;

            InsertRepeatKeys(repeatKeyFilter);
        }

        private void InsertRepeatKeys(ItemGroupRepeatKeyFilterModel repeatKeyFilter)
        {
            switch (repeatKeyFilter.RepeatKeyCounterBasis)
            {
                case RepeatKeyCounterBasis.DrugUnitId:
                    InsertDrugUnitRepeatKeys(repeatKeyFilter);

                    break;

                case RepeatKeyCounterBasis.SubjectVisit:
                    InsertSubjectVisitRepeatKeys(repeatKeyFilter);

                    break;

                case RepeatKeyCounterBasis.ScreenFailReason:
                    InsertScreenFailReasonRepeatKeys(repeatKeyFilter);

                    break;

                default:

                    break;
            }
        }

        private void InsertDrugUnitRepeatKeys(ItemGroupRepeatKeyFilterModel repeatKeyFilter)
        {
            foreach (var drug in repeatKeyFilter.Drugs)
            {
                var drugUnitIdToRestore = itemGroupRepeatKeyHelper.GetDrugUnitIdToRestore(
                    repeatKeyFilter,
                    drug);
                var shouldOverwriteRepeatKey = !drugUnitIdToRestore.IsNullOrEmpty();

                ItemGroupRepeatKeyModel itemGroupRepeatKey = null;

                if (shouldOverwriteRepeatKey)
                {
                    itemGroupRepeatKey = itemGroupRepeatKeyHelper.GetReplacedItemGroupRepeatKey(
                        repeatKeyFilter,
                        drug);
                }
                else
                {
                    itemGroupRepeatKey = itemGroupRepeatKeyHelper.GetMaxItemGroupRepeatKey(
                        repeatKeyFilter,
                        drug);
                }

                var repeatKey = GetRepeatKeyValue(
                    itemGroupRepeatKey.RepeatKey,
                    repeatKeyFilter.RepeatKeyInitialValue,
                    shouldOverwriteRepeatKey);

                var transactionType = shouldOverwriteRepeatKey
                    ? repeatKeyFilter.UpdateTransactionType
                    : null;

                var repeatKeyToAdd = new GenericItemGroupRepeatKeySqlView(
                    repeatKey,
                    repeatKeyFilter,
                    transactionType)
                {
                    DrugUnitId = drug.AssignedDrugUnitId,
                    ReplacedDrugUnitId = drug.ReplacedDrugUnitId,
                    DrugUnitIdToRestore = drugUnitIdToRestore,
                    LogicalSubjectVisitId = itemGroupRepeatKey.LogicalSubjectVisitId,
                    LogicalVisitId = itemGroupRepeatKey.LogicalVisitId,
                    IsSelfSupportDrugUnit = repeatKeyFilter.SelfSupport
                };

                Db.Set<GenericItemGroupRepeatKeySqlView>()
                    .Add(repeatKeyToAdd);

                if (shouldOverwriteRepeatKey)
                {
                    var repeatKeyToUpdate = Db.Set<GenericItemGroupRepeatKeySqlView>()
                        .FirstOrDefault(x => drug.ReplacedDrugUnitId == x.DrugUnitId);

                    if (repeatKeyToUpdate != null)
                    {
                        repeatKeyToUpdate.IsDrugUnitReplaced = true;

                        Db.Set<GenericItemGroupRepeatKeySqlView>()
                            .Update(repeatKeyToUpdate);
                    }
                }
                else
                {
                    var lastUsedRepeatKeyToUpdate = Db.Set<GenericItemGroupRepeatKeyLastUsedSqlView>()
                        .FirstOrDefault(x => itemGroupRepeatKey.RepeatKeyLastUsedId == x.RepeatKeyLastUsedId);

                    if (lastUsedRepeatKeyToUpdate == null)
                    {
                        var lastUsedRepeatKeyToAdd = new GenericItemGroupRepeatKeyLastUsedSqlView(
                            repeatKey,
                            repeatKeyFilter)
                        {
                            LogicalSubjectVisitId = itemGroupRepeatKey.LogicalSubjectVisitId,
                            LogicalVisitId = itemGroupRepeatKey.LogicalVisitId
                        };

                        Db.Set<GenericItemGroupRepeatKeyLastUsedSqlView>()
                            .Add(lastUsedRepeatKeyToAdd);
                    }
                    else
                    {
                        lastUsedRepeatKeyToUpdate.RepeatKeyLastUsed = repeatKey;

                        Db.Set<GenericItemGroupRepeatKeyLastUsedSqlView>()
                            .Update(lastUsedRepeatKeyToUpdate);
                    }
                }

                Db.SaveChanges();
            }
        }

        private void InsertSubjectVisitRepeatKeys(ItemGroupRepeatKeyFilterModel repeatKeyFilter)
        {
            var maxRepeatKey = itemGroupRepeatKeyHelper.GetMaxItemGroupRepeatKey(repeatKeyFilter);

            var repeatKey = GetRepeatKeyValue(
                maxRepeatKey.RepeatKey,
                repeatKeyFilter.RepeatKeyInitialValue);

            var repeatKeyToAdd = new GenericItemGroupRepeatKeySqlView(
                repeatKey,
                repeatKeyFilter)
            {
                LogicalSubjectVisitId = maxRepeatKey.LogicalSubjectVisitId,
                LogicalVisitId = maxRepeatKey.LogicalVisitId
            };

            Db.Set<GenericItemGroupRepeatKeySqlView>()
                .Add(repeatKeyToAdd);

            var lastUsedRepeatKeyToUpdate = Db.Set<GenericItemGroupRepeatKeyLastUsedSqlView>()
                .FirstOrDefault(x => maxRepeatKey.RepeatKeyLastUsedId == x.RepeatKeyLastUsedId);

            if (lastUsedRepeatKeyToUpdate == null)
            {
                var lastUsedRepeatKeyToAdd = new GenericItemGroupRepeatKeyLastUsedSqlView(
                    repeatKey,
                    repeatKeyFilter)
                {
                    LogicalSubjectVisitId = maxRepeatKey.LogicalSubjectVisitId,
                    LogicalVisitId = maxRepeatKey.LogicalVisitId
                };

                Db.Set<GenericItemGroupRepeatKeyLastUsedSqlView>()
                    .Add(lastUsedRepeatKeyToAdd);
            }
            else
            {
                lastUsedRepeatKeyToUpdate.RepeatKeyLastUsed = repeatKey;

                Db.Set<GenericItemGroupRepeatKeyLastUsedSqlView>()
                    .Update(lastUsedRepeatKeyToUpdate);
            }

            Db.SaveChanges();
        }

        private void InsertScreenFailReasonRepeatKeys(ItemGroupRepeatKeyFilterModel repeatKeyFilter)
        {
            var maxRepeatKey = itemGroupRepeatKeyHelper.GetMaxItemGroupRepeatKey(repeatKeyFilter);

            var repeatKey = GetRepeatKeyValue(
                maxRepeatKey.RepeatKey,
                repeatKeyFilter.RepeatKeyInitialValue);

            foreach (var sfReason in repeatKeyFilter.ScreenFailReasons)
            {
                var repeatKeyToAdd = new GenericItemGroupRepeatKeySqlView(
                    repeatKey,
                    repeatKeyFilter)
                {
                    LogicalSubjectVisitId = maxRepeatKey.LogicalSubjectVisitId,
                    LogicalVisitId = maxRepeatKey.LogicalVisitId
                };

                Db.Set<GenericItemGroupRepeatKeySqlView>()
                    .Add(repeatKeyToAdd);

                var lastUsedRepeatKeyToUpdate = Db.Set<GenericItemGroupRepeatKeyLastUsedSqlView>()
                    .FirstOrDefault(x => maxRepeatKey.RepeatKeyLastUsedId == x.RepeatKeyLastUsedId);

                if (lastUsedRepeatKeyToUpdate == null)
                {
                    var lastUsedRepeatKeyToAdd = new GenericItemGroupRepeatKeyLastUsedSqlView(
                        repeatKey,
                        repeatKeyFilter)
                    {
                        LogicalSubjectVisitId = maxRepeatKey.LogicalSubjectVisitId,
                        LogicalVisitId = maxRepeatKey.LogicalVisitId
                    };

                    Db.Set<GenericItemGroupRepeatKeyLastUsedSqlView>()
                        .Add(lastUsedRepeatKeyToAdd);
                }
                else
                {
                    lastUsedRepeatKeyToUpdate.RepeatKeyLastUsed = repeatKey;

                    Db.Set<GenericItemGroupRepeatKeyLastUsedSqlView>()
                        .Update(lastUsedRepeatKeyToUpdate);
                }

                repeatKey++;
            }

            Db.SaveChanges();
        }

        private int GetRepeatKeyValue(
            int? repeatKey,
            int fallback,
            bool shouldOverwrite = false)
        {
            var increment = shouldOverwrite
                ? 0
                : 1;

            var result = repeatKey.HasValue
                ? repeatKey.Value + increment
                : fallback;

            return result;
        }
    }
}
