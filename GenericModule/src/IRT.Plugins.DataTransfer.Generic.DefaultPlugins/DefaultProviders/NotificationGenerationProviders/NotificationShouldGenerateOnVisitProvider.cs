using System;
using System.Linq.Expressions;
using Frameworks.ExtendedProperties.Providers;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using System.Linq;
using IRT.Domain.ViewsSql.Subject;
using System.ComponentModel;
using IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DefaultProviders.NotificationGenerationProviders.Base;

namespace IRT.Plugins.DataTransfer.Generic.DefaultPlugins.DefaultProviders.NotificationGenerationProviders
{
    [Description("Generate on configured Visit - No Self Support")]
    public class NotificationShouldGenerateOnVisitProvider : ShouldGenerateNotificationOnVisitProvider
    {
        public NotificationShouldGenerateOnVisitProvider(
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IQueryable<SubjectSqlView> subjectQuery,
            IQueryable<SubjectVisitSqlView> subjectVisitQuery) : base(extendedPropertiesValueProvider, subjectQuery, subjectVisitQuery)
        {
        }

        protected override Expression<Func<NotificationGenerationSettings, string[]>> VisitIdSelector =>
            settings => settings.VisitId;

        public override bool ShouldGenerateNotification(NotificationGenerationContext context) =>
            base.ShouldGenerateNotification(context)
            && VisitPerformed(context);

        protected virtual bool VisitPerformed(NotificationGenerationContext context)
        {
            switch (context.BaseGenerationContext.IRTEvent)
            {
                case SubjectVisitPerformed _:
                    return true;

                case SelfSupportChangeRequestProcessed _:
                    return false;

                default:
                    return false;
            }
        }
    }
}
