using System.Linq;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDefinition;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDependency;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews
{
    public class DefaultGenericNotificationSqlViewHandler : IntegrationProcessSqlViewHandler
    {
        public DefaultGenericNotificationSqlViewHandler(
            IEventGenerationService eventGenerationService,
            IGenericNotificationDependencyService genericNotificationDependencyService,
            IGenericNotificationDefinitionService genericNotificationDefinitionService)
            : base(eventGenerationService, genericNotificationDependencyService, genericNotificationDefinitionService)
        {
        }

        public void Handle(SubjectVisitPerformed subjectVisitPerformed) =>
            HandleInternal(subjectVisitPerformed);

        public void Handle(SelfSupportChangeRequestProcessed selfSupportChangeRequestProcessed)
        {
            var request = Db.SelfSupportModificationRequests.Single(x => x.RequestId == selfSupportChangeRequestProcessed.RequestId);

            if (request.DataChangeType != IRT.Domain.Aggregates.SelfSupportWorkflowOrchestrator.ValueObjects.SubjectSelfSupportDataChangeType.BackOutTransaction)
            {
                HandleInternal(selfSupportChangeRequestProcessed);
            }
        }
    }
}
