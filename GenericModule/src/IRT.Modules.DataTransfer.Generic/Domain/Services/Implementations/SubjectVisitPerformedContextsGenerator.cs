using System;
using System.Collections.Generic;
using System.Linq;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Domain.ViewsSql.Subject;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts;

namespace IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations
{
    public class SubjectVisitPerformedContextsGenerator : BaseContextsGenerator<SubjectVisitPerformed>
    {
        protected readonly IQueryable<SubjectVisitSqlView> SubjectVisitsQuery;

        public SubjectVisitPerformedContextsGenerator(IQueryable<SubjectVisitSqlView> subjectVisitsQuery)
        {
            SubjectVisitsQuery = subjectVisitsQuery ?? throw new ArgumentNullException(nameof(subjectVisitsQuery));
        }

        protected override BaseGenerationContext GetContextDetails(SubjectVisitPerformed irtEvent)
        {
            var subjecVisitDetails = SubjectVisitsQuery
                .Where(x => x.Id == irtEvent.SubjectVisitId)
                .Select(x => new
                {
                    x.SubjectId,
                    SubjectVisitId = x.Id,
                    x.VisitId,
                    x.Subject.SiteId
                })
                .Single();

            return new BaseGenerationContext(
                subjecVisitDetails.SubjectId,
                subjecVisitDetails.SubjectVisitId,
                subjecVisitDetails.VisitId,
                subjecVisitDetails.SiteId,
                irtEvent,
                ExtractUserId(irtEvent),
                irtEvent.VisitContext);
        }
    }
}