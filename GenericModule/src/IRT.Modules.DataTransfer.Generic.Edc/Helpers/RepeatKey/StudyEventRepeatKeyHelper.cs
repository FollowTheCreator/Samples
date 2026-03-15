using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.StudyEvent;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;

namespace IRT.Modules.DataTransfer.Generic.Edc.Helpers.RepeatKey
{
    public class StudyEventRepeatKeyHelper
    {
        private readonly IQueryable<GenericStudyEventRepeatKeySqlView> genericStudyEventRepeatKeysQuery;
        private readonly IQueryable<GenericStudyEventRepeatKeyLastUsedSqlView> genericStudyEventRepeatKeysLastUsedQuery;

        public StudyEventRepeatKeyHelper(
            IQueryable<GenericStudyEventRepeatKeySqlView> genericStudyEventRepeatKeysQuery,
            IQueryable<GenericStudyEventRepeatKeyLastUsedSqlView> genericStudyEventRepeatKeysLastUsedQuery)
        {
            this.genericStudyEventRepeatKeysQuery = genericStudyEventRepeatKeysQuery;
            this.genericStudyEventRepeatKeysLastUsedQuery = genericStudyEventRepeatKeysLastUsedQuery;
        }

        public MaxStudyEventRepeatKeyModel GetMaxStudyEventRepeatKey(StudyEventRepeatKeyFilterModel repeatKeyFilter)
        {
            Func<GenericStudyEventRepeatKeySqlView, bool> rkBasis = x => true;
            Func<GenericStudyEventRepeatKeyLastUsedSqlView, bool> lastRkBasis = x => true;

            switch (repeatKeyFilter.RepeatKeyBasis)
            {
                case RepeatKeyBasis.SubjectVisit:
                    rkBasis = x => x.SubjectVisitId == repeatKeyFilter.SubjectVisitId;
                    lastRkBasis = x => x.SubjectVisitId == repeatKeyFilter.SubjectVisitId;

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

            var repeatKeys = genericStudyEventRepeatKeysQuery
                .AsNoTracking()
                .Where(rkBasis)
                .Where(x => x.NotificationDefinitionId == repeatKeyFilter.NotificationDefinitionId)
                .ToList();

            var repeatKeyLastUsed = genericStudyEventRepeatKeysLastUsedQuery
                .AsNoTracking()
                .Where(lastRkBasis)
                .Where(x => x.NotificationDefinitionId == repeatKeyFilter.NotificationDefinitionId)
                .FirstOrDefault();

            var repeatKeyLastUsedId = repeatKeyLastUsed?.RepeatKeyLastUsedId;

            if (repeatKeyFilter.ReuseRepeatKey)
            {
                repeatKeyLastUsed = null;
            }

            var result = new MaxStudyEventRepeatKeyModel
            {
                MaxRepeatKey = GetMaxRepeatKey(
                    repeatKeys,
                    repeatKeyLastUsed?.RepeatKeyLastUsed,
                    x => x.RepeatKey),
                MaxScheduledRepeatKey = GetMaxRepeatKey(
                    repeatKeys,
                    repeatKeyLastUsed?.ScheduledRepeatKeyLastUsed,
                    x => x.ScheduledRepeatKey),
                MaxUnscheduledRepeatKey = GetMaxRepeatKey(
                    repeatKeys,
                    repeatKeyLastUsed?.UnscheduledRepeatKeyLastUsed,
                    x => x.UnscheduledRepeatKey),
                MaxReplacementRepeatKey = GetMaxRepeatKey(
                    repeatKeys,
                    repeatKeyLastUsed?.ReplacementRepeatKeyLastUsed,
                    x => x.ReplacementRepeatKey),
                MaxScreenFailRepeatKey = GetMaxRepeatKey(
                    repeatKeys,
                    repeatKeyLastUsed?.ScreenFailRepeatKeyLastUsed,
                    x => x.ScreenFailRepeatKey),
                MaxInformedConsentRepeatKey = GetMaxRepeatKey(
                    repeatKeys,
                    repeatKeyLastUsed?.InformedConsentRepeatKeyLastUsed,
                    x => x.InformedConsentRepeatKey),
                RepeatKeyLastUsedId = repeatKeyLastUsedId
            };

            return result;
        }

        public string GetStudyEventRepeatKeyFormat(
            int? repeatKey,
            GenericEdcNotificationDefinitionSettings genericNotificationDefinitionSettings,
            GenericEdcVisitSettings genericVisitSettings)
        {
            var result = genericVisitSettings?.StudyEventRepeatKeyFormat
                ?? genericNotificationDefinitionSettings.StudyEventRepeatKeyFormat
                ?? null;

            if (genericNotificationDefinitionSettings.StudyEventRepeatKeyCounterInitialValue == repeatKey)
            {
                result = genericVisitSettings?.FirstStudyEventRepeatKeyFormat
                    ?? genericVisitSettings?.StudyEventRepeatKeyFormat
                    ?? genericNotificationDefinitionSettings.FirstStudyEventRepeatKeyFormat
                    ?? genericNotificationDefinitionSettings.StudyEventRepeatKeyFormat
                    ?? null;
            }

            return result;
        }

        private int? GetMaxRepeatKey(
            List<GenericStudyEventRepeatKeySqlView> repeatKeys,
            int? repeatKeyLastUsed,
            Func<GenericStudyEventRepeatKeySqlView, int?> selector)
        {
            var maxRepeatKey = repeatKeys.Any()
                ? repeatKeys
                    .Select(selector)
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
