using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NLog;
using IRT.Domain.Aggregates.SelfSupportWorkflowOrchestrator.ValueObjects;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Domain.Constants;
using IRT.Domain.ViewsSql;
using IRT.Domain.ViewsSql.SelfSupportWorkflows;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations;
using Kernel.DDD.Dispatching;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup
{
    [Priority(EventHandlerPriorityStages.EmailSending)]
    public class GenericItemGroupRepeatKeySqlViewPostHandler : SqlViewHandlerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void Handle(SubjectVisitPerformed subjectVisitPerformed)
        {
        }

        public void Handle(SelfSupportChangeRequestProcessed selfSupportChangeRequestProcessed)
        {
            try
            {
                if (selfSupportChangeRequestProcessed.Status != SelfSupportRequestStatus.Confirmed)
                {
                    return;
                }

                var request = Db.SelfSupportModificationRequests
                    .AsNoTracking()
                    .Include(x => x.Details)
                    .Single(x => x.RequestId == selfSupportChangeRequestProcessed.RequestId);

                if (request.DataChangeType == SubjectSelfSupportDataChangeType.UpdateAssignedDrugs)
                {
                    HandleUpdateAssignedDrugs(request);
                }
                else if (request.DataChangeType == SubjectSelfSupportDataChangeType.BackOutTransaction)
                {
                    HandleBackout(request.SubjectVisitId); //TODO (low priority) decrease the LastUsed RK counter if backout happened for the notification that has not been sent yet
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in the ItemGroup Repeat Key post handler");
            }
        }

        private void HandleUpdateAssignedDrugs(SelfSupportModificationRequestSqlView request)
        {
            var unassignedDrugIds = new List<string>();

            DrugUnitService.ProcessLabeledAssignments(request, null, unassignedDrugIds);
            DrugUnitService.ProcessLabeledReplacements(request, null, unassignedDrugIds);

            var unassignedDrugs = Db.Set<GenericItemGroupRepeatKeySqlView>()
                .Where(x => unassignedDrugIds.Contains(x.DrugUnitId));

            ProcessDeletedDrugs(unassignedDrugs);
        }

        private void HandleBackout(Guid subjectVisitId)
        {
            var backoutDrugs = Db.Set<GenericItemGroupRepeatKeySqlView>()
                .Where(x => x.SubjectVisitId == subjectVisitId);

            ProcessDeletedDrugs(backoutDrugs);
        }

        private void ProcessDeletedDrugs(IQueryable<GenericItemGroupRepeatKeySqlView> drugsToDelete)
        {
            var itemsToUpdate = Db.Set<GenericItemGroupRepeatKeySqlView>()
                .Where(x => drugsToDelete
                    .Select(x => x.ReplacedDrugUnitId)
                    .Contains(x.DrugUnitId))
                .ToList();

            itemsToUpdate
                .ForEach(x =>
                {
                    x.IsDrugUnitReplaced = false;
                });

            Db.Set<GenericItemGroupRepeatKeySqlView>()
                .UpdateRange(itemsToUpdate);

            Db.Set<GenericItemGroupRepeatKeySqlView>()
                .RemoveRange(drugsToDelete);

            Db.SaveChanges();
        }
    }
}
