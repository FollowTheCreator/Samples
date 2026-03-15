using System.Linq;
using IRT.Domain.Aggregates.SelfSupportWorkflowOrchestrator.ValueObjects;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDefinition;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDependency;
using Kernel.DDD.Dispatching;
using Kernel.DDD.Dispatching.Constants;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews
{
    internal class BackoutGenericNotificationHandler : IntegrationProcessSqlViewHandler
    {
        public BackoutGenericNotificationHandler(
            IEventGenerationService eventGenerationService,
            IGenericNotificationDependencyService genericNotificationDependencyService,
            IGenericNotificationDefinitionService genericNotificationDefinitionService)
            : base(eventGenerationService, genericNotificationDependencyService, genericNotificationDefinitionService)
        {
        }

        [Priority(EventHandlerPriorityStages.HandlingBefore)]
        public void Handle(SelfSupportChangeRequestProcessed selfSupportChangeRequestProcessed)
        {
            var request = Db.SelfSupportModificationRequests.Single(x => x.RequestId == selfSupportChangeRequestProcessed.RequestId);

            if (request.DataChangeType == SubjectSelfSupportDataChangeType.BackOutTransaction)
            {
                HandleInternal(selfSupportChangeRequestProcessed);
            }
        }
    }
}
