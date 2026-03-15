using Microsoft.EntityFrameworkCore;
using NLog;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Domain.Constants;
using IRT.Domain.Notifications;
using IRT.Domain.ViewsSql;
using IRT.Domain.ViewsSql.Study;
using IRT.Domain.ViewsSql.Visit;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.GenericDataGeneration.Events;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Edc.Helpers.RepeatKey;
using Kernel.DDD.Dispatching;
using Kernel.Globalization.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.StudyEvent
{
    [Priority(EventHandlerPriorityStages.ProjectedVisitsCalculation)]
    public class GenericStudyEventRepeatKeySqlViewHandler : SqlViewHandlerBase
    {
        private readonly List<VisitSqlView> visitsCache;

        private readonly GenericEdcDataTransferStudySettings genericEdcStudySettings;

        private readonly StudyEventRepeatKeyHelper studyEventRepeatKeyHelper;

        private readonly IEventGenerationService eventGenerationService;

        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public GenericStudyEventRepeatKeySqlViewHandler(
            IQueryable<VisitSqlView> visitsQuery,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            StudyEventRepeatKeyHelper studyEventRepeatKeyHelper,
            IEventGenerationService eventGenerationService)
        {
            visitsCache = visitsQuery.AsNoTracking().ToList();
            genericEdcStudySettings = extendedPropertiesValueProvider.GetValue<StudySqlView, GenericEdcDataTransferStudySettings>(null);
            this.studyEventRepeatKeyHelper = studyEventRepeatKeyHelper;
            this.eventGenerationService = eventGenerationService;
        }

        public void Handle(SubjectVisitPerformed subjectVisitPerformed)
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

                foreach (var eventInfo in eventsInfo)
                {
                    var notificationDefinition = NotificationDefinitionRegistry.GetDefinition(eventInfo.NotificationDefinitionID);

                    var notificationDefinitionSettings = ExtendedPropertiesValueProvider
                         .GetValue<NotificationDefinitionSqlView, GenericEdcNotificationDefinitionSettings>(notificationDefinition.Id.ToString());

                    if (!notificationDefinitionSettings.StudyEventRepeatKeysEnabled)
                    {
                        continue;
                    }

                    var visitInfo = visitsCache.FirstOrDefault(x => x.VisitId == eventInfo.VisitId);

                    var repeatKeyFilter = new StudyEventRepeatKeyFilterModel
                    {
                        NotificationDefinitionId = notificationDefinition.Id,
                        RepeatKeyBasis = notificationDefinitionSettings.StudyEventRepeatKeyBasis,
                        ReuseRepeatKey = notificationDefinitionSettings.AllowStudyEventRepeatKeyReuse,
                        RepeatKeyInitialValue = notificationDefinitionSettings.StudyEventRepeatKeyCounterInitialValue,
                        SubjectId = eventInfo.SubjectId,
                        SubjectVisitId = eventInfo.SubjectVisitId,
                        VisitId = eventInfo.VisitId,
                        IsUnscheduled = visitInfo?.IsUnscheduled ?? false,
                        SiteId = eventInfo.SiteId
                    };

                    InsertRepeatKeys(repeatKeyFilter);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in the StudyEvent Repeat Key SubjectVisitPerformed handler");
            }
        }

        public void Handle(SelfSupportChangeRequestProcessed selfSupportChangeRequestProcessed)
        {
        }

        private void InsertRepeatKeys(StudyEventRepeatKeyFilterModel repeatKeyFilter)
        {
            var maxRepeatKey = studyEventRepeatKeyHelper.GetMaxStudyEventRepeatKey(repeatKeyFilter);

            var repeatKey = GetRepeatKeyValue(maxRepeatKey.MaxRepeatKey, repeatKeyFilter.RepeatKeyInitialValue);

            var scheduledRepeatKey = maxRepeatKey.MaxScheduledRepeatKey;
            if (!repeatKeyFilter.IsUnscheduled
                && !genericEdcStudySettings.ExcludeFromScheduledVisits
                    .Contains(repeatKeyFilter.VisitId))
            {
                scheduledRepeatKey = GetRepeatKeyValue(scheduledRepeatKey, repeatKeyFilter.RepeatKeyInitialValue);
            }

            var unscheduledRepeatKey = maxRepeatKey.MaxUnscheduledRepeatKey;
            if (genericEdcStudySettings.UnscheduledVisits
                .Contains(repeatKeyFilter.VisitId))
            {
                unscheduledRepeatKey = GetRepeatKeyValue(unscheduledRepeatKey, repeatKeyFilter.RepeatKeyInitialValue);
            }

            var replacementRepeatKey = maxRepeatKey.MaxReplacementRepeatKey;
            if (genericEdcStudySettings.ReplacementVisits
                .Contains(repeatKeyFilter.VisitId))
            {
                replacementRepeatKey = GetRepeatKeyValue(replacementRepeatKey, repeatKeyFilter.RepeatKeyInitialValue);
            }

            var screenFailRepeatKey = maxRepeatKey.MaxScreenFailRepeatKey;
            if (genericEdcStudySettings.ScreenFailVisits
                .Contains(repeatKeyFilter.VisitId))
            {
                screenFailRepeatKey = GetRepeatKeyValue(screenFailRepeatKey, repeatKeyFilter.RepeatKeyInitialValue);
            }

            var informedConsentepeatKey = maxRepeatKey.MaxInformedConsentRepeatKey;
            if (genericEdcStudySettings.InformedConsentVisits
                .Contains(repeatKeyFilter.VisitId))
            {
                informedConsentepeatKey = GetRepeatKeyValue(informedConsentepeatKey, repeatKeyFilter.RepeatKeyInitialValue);
            }

            Db.Set<GenericStudyEventRepeatKeySqlView>()
                 .Add(new GenericStudyEventRepeatKeySqlView
                 {
                     RepeatKeyId = Guid.NewGuid(),
                     NotificationDefinitionId = repeatKeyFilter.NotificationDefinitionId,
                     SubjectId = repeatKeyFilter.SubjectId,
                     SubjectVisitId = repeatKeyFilter.SubjectVisitId,
                     SiteId = repeatKeyFilter.SiteId,
                     RepeatKey = repeatKey,
                     ScheduledRepeatKey = scheduledRepeatKey,
                     UnscheduledRepeatKey = unscheduledRepeatKey,
                     ReplacementRepeatKey = replacementRepeatKey,
                     ScreenFailRepeatKey = screenFailRepeatKey,
                     InformedConsentRepeatKey = informedConsentepeatKey
                 });

            var lastUsedRepeatKeyToUpdate = Db.Set<GenericStudyEventRepeatKeyLastUsedSqlView>()
                .FirstOrDefault(x => maxRepeatKey.RepeatKeyLastUsedId == x.RepeatKeyLastUsedId);

            if (lastUsedRepeatKeyToUpdate == null)
            {
                Db.Set<GenericStudyEventRepeatKeyLastUsedSqlView>()
                    .Add(new GenericStudyEventRepeatKeyLastUsedSqlView
                    {
                        RepeatKeyLastUsedId = Guid.NewGuid(),
                        NotificationDefinitionId = repeatKeyFilter.NotificationDefinitionId,
                        SubjectId = repeatKeyFilter.SubjectId,
                        SubjectVisitId = repeatKeyFilter.SubjectVisitId,
                        SiteId = repeatKeyFilter.SiteId,
                        RepeatKeyLastUsed = repeatKey,
                        ScheduledRepeatKeyLastUsed = scheduledRepeatKey,
                        UnscheduledRepeatKeyLastUsed = unscheduledRepeatKey,
                        ReplacementRepeatKeyLastUsed = replacementRepeatKey,
                        ScreenFailRepeatKeyLastUsed = screenFailRepeatKey,
                        InformedConsentRepeatKeyLastUsed = informedConsentepeatKey
                    });
            }
            else
            {
                lastUsedRepeatKeyToUpdate.RepeatKeyLastUsed = repeatKey;
                lastUsedRepeatKeyToUpdate.ScheduledRepeatKeyLastUsed = scheduledRepeatKey;
                lastUsedRepeatKeyToUpdate.UnscheduledRepeatKeyLastUsed = unscheduledRepeatKey;
                lastUsedRepeatKeyToUpdate.ReplacementRepeatKeyLastUsed = replacementRepeatKey;
                lastUsedRepeatKeyToUpdate.ScreenFailRepeatKeyLastUsed = screenFailRepeatKey;
                lastUsedRepeatKeyToUpdate.InformedConsentRepeatKeyLastUsed = informedConsentepeatKey;

                Db.Set<GenericStudyEventRepeatKeyLastUsedSqlView>()
                    .Update(lastUsedRepeatKeyToUpdate);
            }

            Db.SaveChanges();
        }

        private int GetRepeatKeyValue(int? repeatKey, int fallback)
        {
            var result = repeatKey.HasValue
                ? repeatKey.Value + 1
                : fallback;

            return result;
        }
    }
}
