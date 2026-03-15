using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Frameworks.ExtendedProperties.Providers;
using IRT.Domain.Aggregates.SelfSupportWorkflowOrchestrator.ValueObjects;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Domain.ViewsSql.ExtendedProperty;
using IRT.Domain.ViewsSql.SelfSupportWorkflows;
using IRT.Domain.ViewsSql.Subject;
using IRT.Domain.ViewsSql.SubjectOperationLog;
using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;

namespace IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DefaultProviders.NotificationGenerationProviders
{

    [Description("Generate on configured Visit and configured Self Support data points")]
    public class NotificationShouldGenerateOnVisitAndSelfSupportProvider : NotificationShouldGenerateOnVisitProvider
    {
        protected readonly IQueryable<SelfSupportModificationRequestSqlView> SelfSupportRequestsQuery;

        public NotificationShouldGenerateOnVisitAndSelfSupportProvider(
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IQueryable<SelfSupportModificationRequestSqlView> selfSupportRequestsQuery,
            IQueryable<SubjectSqlView> subjectQuery,
            IQueryable<SubjectVisitSqlView> subjectVisitQuery) : base(extendedPropertiesValueProvider, subjectQuery, subjectVisitQuery)
        {
            SelfSupportRequestsQuery = selfSupportRequestsQuery;
        }

        public override bool ShouldGenerateNotification(NotificationGenerationContext context) =>
            VisitPerformed(context);

        protected override bool VisitPerformed(NotificationGenerationContext context)
        {
            switch (context.BaseGenerationContext.IRTEvent)
            {
                case SubjectVisitPerformed _:
                    return IsCurrentVisitExpected(context);

                case SelfSupportChangeRequestProcessed selfSupportChangeRequestProcessed:
                    return IsChangeKindUpdate(selfSupportChangeRequestProcessed, context)
                            && WasExpectedVisitPerformed(context)
                           || IsBackout(selfSupportChangeRequestProcessed, context)
                               && IsCurrentVisitExpected(context);

                default:
                    return false;
            }
        }

        private bool IsBackout(SelfSupportChangeRequestProcessed selfSupportChangeRequestProcessed, NotificationGenerationContext context)
        {
            var configuredSelfSupportActionsRaw = GetSelfSupportActionsRaw(context);

            if (configuredSelfSupportActionsRaw == null || !configuredSelfSupportActionsRaw.Any())
            {
                return false;
            }

            var request = SelfSupportRequestsQuery
                .Where(x => x.RequestId == selfSupportChangeRequestProcessed.RequestId)
                .Include(x => x.Details)
                .Single();

            // convert strings into SubjectSelfSupportDataChangeType
            var configuredSelfSupportActions = ConvertSelfSupportChangeTypes(configuredSelfSupportActionsRaw);

            return configuredSelfSupportActions.Contains(request.DataChangeType)
                    && request.DataChangeType == SubjectSelfSupportDataChangeType.BackOutTransaction;
        }

        private IEnumerable<string> GetChangeKindsRaw(NotificationGenerationContext context)
        {
            return ExtendedPropertiesValueProvider.GetValue<EntitySqlView, NotificationGenerationSettings, IEnumerable<string>>
                (x => x.SubjectVisitCapturedDataPoints, context.NotificationEntityId);
        }

        private bool IsChangeKindUpdate(SelfSupportChangeRequestProcessed selfSupportChangeRequestProcessed, NotificationGenerationContext context)
        {
            var configuredChangeKindsRaw = GetChangeKindsRaw(context);

            var configuredSelfSupportActionsRaw = GetSelfSupportActionsRaw(context);

            if (configuredSelfSupportActionsRaw == null || !configuredSelfSupportActionsRaw.Any())
            {
                return false;
            }

            var request = SelfSupportRequestsQuery
                .Where(x => x.RequestId == selfSupportChangeRequestProcessed.RequestId)
                .Include(x => x.Details)
                .Single();

            // convert strings into SubjectOperationLogChangeKind
            var configuredChangeKinds = ConvertChangeKinds(configuredChangeKindsRaw);

            // convert strings into SubjectSelfSupportDataChangeType
            var configuredSelfSupportActions = ConvertSelfSupportChangeTypes(configuredSelfSupportActionsRaw);

            return request.HasAnyBeenChanged(configuredChangeKinds.ToArray())
                    && configuredSelfSupportActions.Contains(request.DataChangeType)
                    && request.DataChangeType != SubjectSelfSupportDataChangeType.BackOutTransaction;
        }

        private IEnumerable<string> GetSelfSupportActionsRaw(NotificationGenerationContext context)
        {
            return ExtendedPropertiesValueProvider.GetValue<EntitySqlView, NotificationGenerationSettings, IEnumerable<string>>
                (x => x.SelfSupportActions, context.NotificationEntityId);
        }

        private List<SubjectOperationLogChangeKind> ConvertChangeKinds(IEnumerable<string> configuredChangeKindsRaw)
        {
            // convert strings into SubjectOperationLogChangeKind
            var configuredChangeKinds = new List<SubjectOperationLogChangeKind>();
            if (configuredChangeKindsRaw != null && configuredChangeKindsRaw.Any())
            {
                foreach (var changeKindName in configuredChangeKindsRaw)
                {
                    configuredChangeKinds.Add(SubjectOperationLogChangeKind.GetChangeKindFromName(changeKindName));
                }
            }

            return configuredChangeKinds;
        }

        private List<SubjectSelfSupportDataChangeType> ConvertSelfSupportChangeTypes(IEnumerable<string> configuredSelfSupportActionsRaw)
        {
            // convert strings into SubjectSelfSupportDataChangeType
            var configuredSelfSupportActions = new List<SubjectSelfSupportDataChangeType>();
            foreach (var selfSupportAction in configuredSelfSupportActionsRaw)
            {
                configuredSelfSupportActions.Add((SubjectSelfSupportDataChangeType)System.Enum.Parse(typeof(SubjectSelfSupportDataChangeType), selfSupportAction));
            }

            return configuredSelfSupportActions;
        }
    }
}
