using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Frameworks.ExtendedProperties.Providers;
using IRT.Domain.ViewsSql.Study;
using IRT.Domain.ViewsSql.Subject.SubjectVisitDrug;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Helpers.RepeatKey
{
    public class ItemGroupRepeatKeyHelper
    {
        private readonly IQueryable<GenericItemGroupRepeatKeySqlView> genericItemGroupRepeatKeysQuery;
        private readonly IQueryable<GenericItemGroupRepeatKeyLastUsedSqlView> genericItemGroupRepeatKeysLastUsedQuery;
        private readonly IQueryable<SubjectVisitLabeledDrugSqlView> subjectVisitLabeledDrugsQuery;

        private readonly GenericEdcDataTransferStudySettings genericEdcStudySettings;

        public ItemGroupRepeatKeyHelper(
            IQueryable<GenericItemGroupRepeatKeySqlView> genericItemGroupRepeatKeysQuery,
            IQueryable<GenericItemGroupRepeatKeyLastUsedSqlView> genericItemGroupRepeatKeysLastUsedQuery,
            IQueryable<SubjectVisitLabeledDrugSqlView> subjectVisitLabeledDrugsQuery,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider)
        {
            this.genericItemGroupRepeatKeysQuery = genericItemGroupRepeatKeysQuery;
            this.genericItemGroupRepeatKeysLastUsedQuery = genericItemGroupRepeatKeysLastUsedQuery;
            this.subjectVisitLabeledDrugsQuery = subjectVisitLabeledDrugsQuery;

            genericEdcStudySettings = extendedPropertiesValueProvider.GetValue<StudySqlView, GenericEdcDataTransferStudySettings>(null);
        }

        public ItemGroupRepeatKeyModel GetMaxItemGroupRepeatKey(ItemGroupRepeatKeyFilterModel repeatKeyFilter)
        {
            var logicalSubjectVisitInfo = GetLogicalSubjectVisitInfo(repeatKeyFilter);

            Func<GenericItemGroupRepeatKeySqlView, bool> rkBasis = x => true;
            Func<GenericItemGroupRepeatKeyLastUsedSqlView, bool> lastRkBasis = x => true;

            switch (repeatKeyFilter.RepeatKeyBasis)
            {
                case RepeatKeyBasis.SubjectVisit:
                    rkBasis = x => x.LogicalVisitId == logicalSubjectVisitInfo.LogicalVisitId
                        && x.SubjectId == repeatKeyFilter.SubjectId;
                    lastRkBasis = x => x.LogicalVisitId == logicalSubjectVisitInfo.LogicalVisitId
                        && x.SubjectId == repeatKeyFilter.SubjectId;

                    break;

                case RepeatKeyBasis.Subject:
                    rkBasis = x => x.SubjectId == repeatKeyFilter.SubjectId;
                    lastRkBasis = x => x.SubjectId == repeatKeyFilter.SubjectId;

                    break;

                case RepeatKeyBasis.Site:
                    rkBasis = x => x.SiteId == repeatKeyFilter.SiteId;
                    lastRkBasis = x => x.SiteId == repeatKeyFilter.SiteId;

                    break;

                default:
                    rkBasis = x => true;
                    lastRkBasis = x => true;

                    break;
            }

            var repeatKeys = genericItemGroupRepeatKeysQuery
                .AsNoTracking()
                .Where(rkBasis)
                .Where(x => x.NotificationDefinitionId == repeatKeyFilter.NotificationDefinitionId)
                .ToList();

            var repeatKeyLastUsed = genericItemGroupRepeatKeysLastUsedQuery
                .AsNoTracking()
                .Where(lastRkBasis)
                .Where(x => x.NotificationDefinitionId == repeatKeyFilter.NotificationDefinitionId)
                .FirstOrDefault();

            var repeatKeyLastUsedId = repeatKeyLastUsed?.RepeatKeyLastUsedId;

            if (repeatKeyFilter.ReuseRepeatKey)
            {
                repeatKeyLastUsed = null;
            }

            var result = new ItemGroupRepeatKeyModel
            {
                RepeatKey = GetMaxRepeatKey(
                    repeatKeys,
                    repeatKeyLastUsed?.RepeatKeyLastUsed),
                RepeatKeyLastUsedId = repeatKeyLastUsedId,
                LogicalSubjectVisitId = logicalSubjectVisitInfo.LogicalSubjectVisitId,
                LogicalVisitId = logicalSubjectVisitInfo.LogicalVisitId
            };

            return result;
        }

        public ItemGroupRepeatKeyModel GetMaxItemGroupRepeatKey(
            ItemGroupRepeatKeyFilterModel repeatKeyFilter,
            DrugInfo drug)
        {
            var logicalSubjectVisitInfo = GetLogicalSubjectVisitInfo(repeatKeyFilter, drug);

            Func<GenericItemGroupRepeatKeySqlView, bool> rkBasis = x => true;
            Func<GenericItemGroupRepeatKeyLastUsedSqlView, bool> lastRkBasis = x => true;

            switch (repeatKeyFilter.RepeatKeyBasis)
            {
                case RepeatKeyBasis.SubjectVisit:
                    rkBasis = x => x.LogicalVisitId == logicalSubjectVisitInfo.LogicalVisitId
                        && x.SubjectId == repeatKeyFilter.SubjectId;
                    lastRkBasis = x => x.LogicalVisitId == logicalSubjectVisitInfo.LogicalVisitId
                        && x.SubjectId == repeatKeyFilter.SubjectId;

                    break;

                case RepeatKeyBasis.Subject:
                    rkBasis = x => x.SubjectId == repeatKeyFilter.SubjectId;
                    lastRkBasis = x => x.SubjectId == repeatKeyFilter.SubjectId;

                    break;

                case RepeatKeyBasis.Site:
                    rkBasis = x => x.SiteId == repeatKeyFilter.SiteId;
                    lastRkBasis = x => x.SiteId == repeatKeyFilter.SiteId;

                    break;

                default:
                    rkBasis = x => true;
                    lastRkBasis = x => true;

                    break;
            }

            var repeatKeys = genericItemGroupRepeatKeysQuery
                .AsNoTracking()
                .Where(rkBasis)
                .Where(x => x.NotificationDefinitionId == repeatKeyFilter.NotificationDefinitionId)
                .ToList();

            var repeatKeyLastUsed = genericItemGroupRepeatKeysLastUsedQuery
                .AsNoTracking()
                .Where(lastRkBasis)
                .Where(x => x.NotificationDefinitionId == repeatKeyFilter.NotificationDefinitionId)
                .FirstOrDefault();

            var repeatKeyLastUsedId = repeatKeyLastUsed?.RepeatKeyLastUsedId;

            if (repeatKeyFilter.ReuseRepeatKey)
            {
                repeatKeyLastUsed = null;
            }

            var result = new ItemGroupRepeatKeyModel
            {
                RepeatKey = GetMaxRepeatKey(
                    repeatKeys,
                    repeatKeyLastUsed?.RepeatKeyLastUsed),
                RepeatKeyLastUsedId = repeatKeyLastUsedId,
                LogicalSubjectVisitId = logicalSubjectVisitInfo.LogicalSubjectVisitId,
                LogicalVisitId = logicalSubjectVisitInfo.LogicalVisitId
            };

            return result;
        }

        public ItemGroupRepeatKeyModel GetReplacedItemGroupRepeatKey(
            ItemGroupRepeatKeyFilterModel repeatKeyFilter,
            DrugInfo drug)
        {
            var itemGroupRepeatKey = genericItemGroupRepeatKeysQuery
                .AsNoTracking()
                .Where(x => x.DrugUnitId == drug.ReplacedDrugUnitId
                    && x.NotificationDefinitionId == repeatKeyFilter.NotificationDefinitionId)
                .Select(x => new
                {
                    x.RepeatKey,
                    x.LogicalSubjectVisitId,
                    x.LogicalVisitId
                })
                .FirstOrDefault();

            var result = new ItemGroupRepeatKeyModel
            {
                RepeatKey = itemGroupRepeatKey.RepeatKey,
                LogicalSubjectVisitId = itemGroupRepeatKey.LogicalSubjectVisitId,
                LogicalVisitId = itemGroupRepeatKey.LogicalVisitId,
            };

            return result;
        }

        public LogicalSubjectVisitInfo GetLogicalSubjectVisitInfo(
            ItemGroupRepeatKeyFilterModel repeatKeyFilter,
            DrugInfo drug = null)
        {
            var result = new LogicalSubjectVisitInfo
            {
                LogicalSubjectVisitId = repeatKeyFilter.SubjectVisitId,
                LogicalVisitId = repeatKeyFilter.VisitId
            };

            if (genericEdcStudySettings.UnscheduledVisits.Contains(repeatKeyFilter.VisitId))
            {
                if (repeatKeyFilter.UnscheduledVisitsStrategy == UnscheduledVisitsStrategy.ParentSubjectVisit)
                {
                    result.LogicalSubjectVisitId = repeatKeyFilter.ParentSubjectVisitId;
                    result.LogicalVisitId = repeatKeyFilter.ParentVisitId;
                }
            }
            else if (genericEdcStudySettings.ReplacementVisits.Contains(repeatKeyFilter.VisitId))
            {
                switch (repeatKeyFilter.ReplacementVisitsStrategy)
                {
                    case ReplacementVisitsStrategy.ParentSubjectVisit:
                        result.LogicalSubjectVisitId = repeatKeyFilter.ParentSubjectVisitId;
                        result.LogicalVisitId = repeatKeyFilter.ParentVisitId;

                        break;

                    case ReplacementVisitsStrategy.ReplacedDrugUnitId:
                        if (drug != null)
                        {
                            if (drug.ReplacedDrugUnitId != null)
                            {
                                var replacedInfo = genericItemGroupRepeatKeysQuery
                                    .Where(x => x.DrugUnitId == drug.ReplacedDrugUnitId
                                        && x.NotificationDefinitionId == repeatKeyFilter.NotificationDefinitionId)
                                    .Select(x => new
                                    {
                                        x.LogicalSubjectVisitId,
                                        x.LogicalVisitId
                                    })
                                    .FirstOrDefault();

                                result.LogicalSubjectVisitId = replacedInfo.LogicalSubjectVisitId;
                                result.LogicalVisitId = replacedInfo.LogicalVisitId;
                            }
                            else
                            {
                                result.LogicalSubjectVisitId = repeatKeyFilter.ParentSubjectVisitId;
                                result.LogicalVisitId = repeatKeyFilter.ParentVisitId;
                            }
                        }

                        break;

                    default:

                        break;
                }
            }

            return result;
        }

        public string GetDrugUnitIdToRestore(
            ItemGroupRepeatKeyFilterModel repeatKeyFilter,
            DrugInfo drug)
        {
            if (drug.ReplacedDrugUnitId.IsNullOrEmpty())
            {
                return null;
            }

            if ((!repeatKeyFilter.SelfSupport
                    && genericEdcStudySettings.ReplacementVisits.Contains(repeatKeyFilter.VisitId)
                    && repeatKeyFilter.ReplacementVisitsStrategy == ReplacementVisitsStrategy.ReplacedDrugUnitId
                    && repeatKeyFilter.OverwriteReplacedDrug)
                || (repeatKeyFilter.SelfSupport
                    && repeatKeyFilter.OverwriteManuallyReplacedDrug))
            {
                return drug.ReplacedDrugUnitId;
            }

            return null;
        }

        public string GetItemGroupRepeatKeyFormat(
            int? repeatKey,
            GenericEdcNotificationDefinitionSettings genericNotificationDefinitionSettings,
            GenericEdcVisitSettings genericVisitSettings)
        {
            var result = genericVisitSettings?.ItemGroupRepeatKeyFormat
                ?? genericNotificationDefinitionSettings.ItemGroupRepeatKeyFormat
                ?? null;

            if (genericNotificationDefinitionSettings.ItemGroupRepeatKeyCounterInitialValue == repeatKey)
            {
                result = genericVisitSettings?.FirstItemGroupRepeatKeyFormat
                    ?? genericVisitSettings?.ItemGroupRepeatKeyFormat
                    ?? genericNotificationDefinitionSettings.FirstItemGroupRepeatKeyFormat
                    ?? genericNotificationDefinitionSettings.ItemGroupRepeatKeyFormat
                    ?? null;
            }

            return result;
        }

        public string GetItemGroupRepeatKeyFormat(
            string repeatKey,
            GenericEdcNotificationDefinitionSettings genericNotificationDefinitionSettings,
            GenericEdcVisitSettings genericVisitSettings)
        {
            if (int.TryParse(repeatKey, out var repeatKeyInt))
            {
                return GetItemGroupRepeatKeyFormat(
                    repeatKeyInt,
                    genericNotificationDefinitionSettings,
                    genericVisitSettings);
            }

            return genericNotificationDefinitionSettings.ItemGroupRepeatKeyFormat;
        }

        private int? GetMaxRepeatKey(
            List<GenericItemGroupRepeatKeySqlView> repeatKeys,
            int? repeatKeyLastUsed)
        {
            var maxRepeatKey = repeatKeys.Any()
                ? repeatKeys
                    .Select(x => x.RepeatKey)
                    .Max()
                : null;

            if (maxRepeatKey.HasValue && repeatKeyLastUsed.HasValue)
            {
                return Math.Max(maxRepeatKey.Value, repeatKeyLastUsed.Value);
            }

            return maxRepeatKey
                ?? repeatKeyLastUsed;
        }
    }
}
