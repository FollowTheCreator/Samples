using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Frameworks.ExtendedProperties.Dto;
using Frameworks.ExtendedProperties.Providers;
using IRT.Domain;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Domain.ViewsSql.ExtendedProperty;
using IRT.Domain.ViewsSql.SelfSupportWorkflows;
using IRT.Domain.ViewsSql.Site;
using IRT.Domain.ViewsSql.Subject;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.GenericDataGeneration.Events;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.Dto;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using Kernel.Infrastructure.DateTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations
{
    public class EventGenerationService : IEventGenerationService
    {
        private readonly IContextsGeneratorFactory contextsGeneratorFactory;
        private readonly IExtendedPropertiesValueProvider extendedPropertiesValueProvider;
        private readonly INotificationGenerationService notificationGenerationService;
        protected readonly IDateTimeService dateTimeService;
        protected readonly IQueryable<SiteSqlView> sitesQuery;
        protected readonly IQueryable<SelfSupportModificationRequestSqlView> requestsQuery;
        protected readonly IQueryable<SubjectSqlView> subjectsQuery;
        protected readonly IQueryable<SubjectVisitSqlView> subjectVisitsQuery;

        public EventGenerationService(IContextsGeneratorFactory contextsGeneratorFactory,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            INotificationGenerationService notificationGenerationService,
            IDateTimeService dateTimeService,
            IQueryable<SiteSqlView> sitesQuery,
            IQueryable<SelfSupportModificationRequestSqlView> requestsQuery,
            IQueryable<SubjectSqlView> subjectsQuery,
            IQueryable<SubjectVisitSqlView> subjectVisitsQuery)
        {
            this.contextsGeneratorFactory = contextsGeneratorFactory;
            this.extendedPropertiesValueProvider = extendedPropertiesValueProvider;
            this.notificationGenerationService = notificationGenerationService;
            this.dateTimeService = dateTimeService;
            this.sitesQuery = sitesQuery;
            this.requestsQuery = requestsQuery;
            this.subjectsQuery = subjectsQuery;
            this.subjectVisitsQuery = subjectVisitsQuery;
        }

        public IEnumerable<GenericIntegrationDataEvent> CreateGenericDataEvent(IRTEvent irtEvent)
        {
            var contextsGenerator = contextsGeneratorFactory.Create(irtEvent);
            var generationContext = contextsGenerator.GetGenerationContext(irtEvent);
            var notificationSettings = extendedPropertiesValueProvider
                .GetEntities<EntitySqlView, NotificationGenerationSettings>().OrderBy(x => x.Value.GenerationOrder);

            foreach (var setting in notificationSettings)
            {
                var notificationGenerationContext = new NotificationGenerationContext(generationContext, setting.EntityId);
                if (notificationGenerationService.ShouldGenerateNotification(notificationGenerationContext, setting))
                {
                    yield return GenerateNotification(generationContext, setting, irtEvent);
                }
            }
        }

        public GenericIntegrationDataEvent GenerateNotification(
            BaseGenerationContext baseGenerationContext,
            ExtendedPropertyEntityDto<NotificationGenerationSettings> notificationSettings,
            IRTEvent irtEvent)
        {
            var siteData = baseGenerationContext.SiteId;
            var currentUtcDateTime = dateTimeService.GetCurrentUtcDateTime();
            var currentLocalDateTime = dateTimeService.GetCurrentLocalDateTime(
                sitesQuery
                    .Include(x => x.DefaultLocation)
                    .Single(x => x.SiteId == siteData));

            var notificationdefiniton = Guid.Empty;
            var definitionConversion =
                Guid.TryParse(notificationSettings.Value.NotificationDefinition, out notificationdefiniton);
            var additionalInfo = (string)null;
            var visitBackout = false;
            var subjectBackout = false;
            SelfSupportModificationRequestSqlView request = null;

            var selfSupportEvent = baseGenerationContext.IRTEvent as SelfSupportChangeRequestProcessed;
            if (selfSupportEvent is not null)
            {
                request = requestsQuery
                    .Include(x => x.Details)
                    .Single(x => x.RequestId == selfSupportEvent.RequestId);

                var subject = subjectsQuery.Single(x => x.SubjectId == request.SubjectId);

                visitBackout = request.DataChangeType == IRT.Domain.Aggregates.SelfSupportWorkflowOrchestrator
                    .ValueObjects.SubjectSelfSupportDataChangeType.BackOutTransaction;
                subjectBackout = IsScreeningVisitForBackout(request.SubjectId) && visitBackout;
                var subjectInfoOnBackout = new SubjectInfoOnBackout
                {
                    SubjectId = request.SubjectId,
                    SubjectNumber = subject.SubjectNumber,
                    ScreeningNumber = subject.ScreeningNumber,
                    SubjectVisitId = request.SubjectVisitId
                };

                // if the event is a backout, we will track a reference to the subjectid, subject number and subject visit id here
                additionalInfo = visitBackout ? JsonConvert.SerializeObject(subjectInfoOnBackout) : null;
            }

            var genericIntegrationDataEvent = new GenericIntegrationDataEvent()
            {
                SubjectId = baseGenerationContext.SubjectId,
                SubjectVisitId = baseGenerationContext.SubjectVisitId,
                VisitId = baseGenerationContext.VisitId,
                SiteId = baseGenerationContext.SiteId,
                UserId = baseGenerationContext.UserId,
                AdditionalInfo = additionalInfo,
                NotificationDefinitionID = notificationdefiniton,
                TransactionLocalDateTime = currentLocalDateTime,
                TransactionUtcDateTime = currentUtcDateTime,
                VisitContext = baseGenerationContext.VisitContext,
                VisitBackout = visitBackout,
                SubjectBackout = subjectBackout,
                SelfSupportModificationRequest = request,
                NotificationGenerationSettingEntity = notificationSettings
            };

            if (irtEvent is SubjectVisitPerformed visitPerformedEvent)
            {
                genericIntegrationDataEvent.CapturedData = visitPerformedEvent.CapturedData;
            }

            genericIntegrationDataEvent.SetCommandMetadata(irtEvent.CommandMetadata);
            return genericIntegrationDataEvent;
        }

        private bool IsScreeningVisitForBackout(Guid subjectId)
        {
            // used for backouts to determine if visit being backed out is the first visit so that the notification to be saved will not be deleted
            return subjectVisitsQuery.Where(x => x.SubjectId == subjectId).Count() == 1;
        }
    }
}
