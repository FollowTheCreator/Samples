using System;
using System.Linq;
using NLog;
using IRT.Domain.Aggregates.SelfSupportWorkflowOrchestrator.ValueObjects;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Domain.Constants;
using IRT.Domain.ViewsSql;
using Kernel.DDD.Dispatching;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.StudyEvent
{
    [Priority(EventHandlerPriorityStages.EmailSending)]
    public class GenericStudyEventRepeatKeySqlViewPostHandler : SqlViewHandlerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void Handle(SubjectVisitPerformed subjectVisitPerformed)
        {
        }

        public void Handle(SelfSupportChangeRequestProcessed selfSupportChangeRequestProcessed)
        {
            try
            {
                var request = Db.SelfSupportModificationRequests.Single(x => x.RequestId == selfSupportChangeRequestProcessed.RequestId);

                if (request.DataChangeType == SubjectSelfSupportDataChangeType.BackOutTransaction)
                {
                    DeleteRepeatKeys(request.SubjectVisitId); //TODO (low priority) decrease the LastUsed RK counter if backout happened for the notification that has not been sent yet
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in the StudyEvent Repeat Key SelfSupportChangeRequestProcessed post handler");
            }
        }

        private void DeleteRepeatKeys(Guid subjectVisitId)
        {
            var itemsToDelete = Db.Set<GenericStudyEventRepeatKeySqlView>()
                .Where(x => x.SubjectVisitId == subjectVisitId);

            Db.Set<GenericStudyEventRepeatKeySqlView>()
                .RemoveRange(itemsToDelete);

            Db.SaveChanges();
        }
    }
}
