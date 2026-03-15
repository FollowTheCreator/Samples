using System;
using System.Linq;
using System.Linq.Expressions;
using Frameworks.ExtendedProperties.Providers;
using IRT.Domain.ViewsSql.ExtendedProperty;
using IRT.Domain.ViewsSql.Subject;
using IRT.Modules.DataTransfer.Generic.Domain.Providers.ShouldGenerateNotificationProviders.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;

namespace IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DefaultProviders.NotificationGenerationProviders.Base
{
    public abstract class ShouldGenerateNotificationOnVisitProvider : IShouldGenerateNotificationPolicy
    {
        protected readonly IExtendedPropertiesValueProvider ExtendedPropertiesValueProvider;
        protected readonly IQueryable<SubjectSqlView> SubjectQuery;
        protected readonly IQueryable<SubjectVisitSqlView> SubjectVisitQuery;

        protected ShouldGenerateNotificationOnVisitProvider(IExtendedPropertiesValueProvider extendedPropertiesValueProvider
            , IQueryable<SubjectSqlView> subjectQuery
            , IQueryable<SubjectVisitSqlView> subjectVisitQuery)
        {
            ExtendedPropertiesValueProvider = extendedPropertiesValueProvider
                                              ?? throw new ArgumentNullException(nameof(extendedPropertiesValueProvider));
            SubjectQuery = subjectQuery
                           ?? throw new ArgumentNullException(nameof(subjectQuery));
            SubjectVisitQuery = subjectVisitQuery
                                ?? throw new ArgumentNullException(nameof(subjectVisitQuery));
        }

        protected abstract Expression<Func<NotificationGenerationSettings, string[]>> VisitIdSelector { get; }

        public virtual bool ShouldGenerateNotification(NotificationGenerationContext context)
        {
            return IsCurrentVisitExpected(context);
        }

        public bool IsCurrentVisitExpected(NotificationGenerationContext context)
        {
            // for standard visit generation, only checking to see if the current visit matches an expected visit trigger
            var expectedVisitIdForForm = GetVisitIdsForThisForm(context.NotificationEntityId);

            var actualVisitId = context.BaseGenerationContext.VisitId;

            return expectedVisitIdForForm != null
                   && expectedVisitIdForForm.Contains(actualVisitId);
        }

        protected virtual string[] GetVisitIdsForThisForm(string entityId) =>
            ExtendedPropertiesValueProvider.GetValue<EntitySqlView, NotificationGenerationSettings, string[]>(VisitIdSelector, entityId);

        public bool WasExpectedVisitPerformed(NotificationGenerationContext context)
        {
            // check if an expected visit for the form generation context has been performed
            var expectedVisitIdForForm = GetVisitIdsForThisForm(context.NotificationEntityId);

            return SubjectVisitQuery.Any(x => expectedVisitIdForForm.Contains(x.VisitId)
                                              && x.SubjectId == context.BaseGenerationContext.SubjectId);
        }
    }
}
