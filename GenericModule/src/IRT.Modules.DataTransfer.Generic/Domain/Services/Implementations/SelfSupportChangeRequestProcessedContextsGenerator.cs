using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IRT.Domain.Aggregates.SelfSupportWorkflowOrchestrator.ValueObjects;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Domain.ViewsSql.SelfSupportWorkflows;
using IRT.Domain.ViewsSql.Subject;
using IRT.Domain.ViewsSql.SubjectOperationLog;
using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations
{
    public class SelfSupportChangeRequestProcessedContextsGenerator : BaseContextsGenerator<SelfSupportChangeRequestProcessed>
    {
        protected readonly IQueryable<SubjectVisitSqlView> SubjectVisitsQuery;
        protected readonly IQueryable<SubjectSqlView> SubjectsQuery;
        protected readonly IQueryable<SelfSupportModificationRequestSqlView> SelfSupportModificationRequestsQuery;

        public SelfSupportChangeRequestProcessedContextsGenerator(
            IQueryable<SubjectVisitSqlView> subjectVisitsQuery,
            IQueryable<SubjectSqlView> subjectsQuery,
            IQueryable<SelfSupportModificationRequestSqlView> selfSupportModificationRequestsQuery)
        {
            SubjectVisitsQuery = subjectVisitsQuery
                ?? throw new ArgumentNullException(nameof(subjectVisitsQuery));
            SubjectsQuery = subjectsQuery
                ?? throw new ArgumentNullException(nameof(subjectsQuery));
            SelfSupportModificationRequestsQuery = selfSupportModificationRequestsQuery
                ?? throw new ArgumentNullException(nameof(selfSupportModificationRequestsQuery));
        }

        protected override BaseGenerationContext GetContextDetails(SelfSupportChangeRequestProcessed irtEvent)
        {
            var subjecVisitDetails = GetSubjectVisitDetails(irtEvent);

            return new BaseGenerationContext(
                subjecVisitDetails.SubjectId,
                subjecVisitDetails.SubjectVisitId,
                subjecVisitDetails.VisitId,
                subjecVisitDetails.SiteId,
                irtEvent,
                ExtractUserId(irtEvent));
        }

        protected virtual SubjectVisitDetails GetSubjectVisitDetails(SelfSupportChangeRequestProcessed irtEvent)
        {
            var request = SelfSupportModificationRequestsQuery
                .Include(x => x.Details)
                .Where(x => x.RequestId == irtEvent.RequestId)
                .Single();

            switch (request.DataChangeType)
            {
                case SubjectSelfSupportDataChangeType.BackOutTransaction:
                    return ExtractSubjectVisitDetailsOnTransactionBackOut(request);

                default:
                    return ExtractSubjectVisitDetailsWithDefaultBehaviour(request);
            }
        }

        private SubjectVisitDetails ExtractSubjectVisitDetailsWithDefaultBehaviour(
            SelfSupportModificationRequestSqlView request)
        {
            var subjecVisitDetails = SubjectVisitsQuery
                .Where(x => x.Id == request.SubjectVisitId)
                .Select(x => new SubjectVisitDetails
                {
                    SubjectId = x.SubjectId,
                    SubjectVisitId = x.Id,
                    VisitId = x.VisitId,
                    SiteId = x.Subject.SiteId
                })
                .Single();

            return subjecVisitDetails;
        }

        private SubjectVisitDetails ExtractSubjectVisitDetailsOnTransactionBackOut(
            SelfSupportModificationRequestSqlView request)
        {
            var subjecVisitDetails = SubjectVisitsQuery
                .Where(x => x.Id == request.SubjectVisitId)
                .Select(x => new SubjectVisitDetails
                {
                    SubjectId = x.SubjectId,
                    SubjectVisitId = x.Id,
                    VisitId = x.VisitId,
                    SiteId = x.Subject.SiteId
                })
                .SingleOrDefault();

            if (subjecVisitDetails != null)
            {
                return subjecVisitDetails;
            }

            subjecVisitDetails = SubjectsQuery
                .Where(x => x.SubjectId == request.SubjectId)
                .Select(x => new SubjectVisitDetails
                {
                    SubjectId = x.SubjectId,
                    SiteId = x.SiteId
                })
                .SingleOrDefault();

            subjecVisitDetails.SubjectVisitId = request.SubjectVisitId;
            subjecVisitDetails.VisitId = ExtractVisitIdOnBackOut(request);

            return subjecVisitDetails;
        }

        protected virtual string ExtractVisitIdOnBackOut(SelfSupportModificationRequestSqlView request)
        {
            var oldVisitId = request.GetOldValue<string>(SubjectOperationLogChangeKind.VisitId);


            return oldVisitId;
        }

        protected class SubjectVisitDetails
        {
            public Guid SubjectId { get; set; }

            public Guid SubjectVisitId { get; set; }

            public string VisitId { get; set; }

            public string SiteId { get; set; }
        }
    }
}