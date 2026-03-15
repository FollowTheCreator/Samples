using System;
using IRT.Domain;
using Kernel.SharedDomain.ValueObjects.Visit;

namespace IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.GenerationContexts
{
    public class BaseGenerationContext
    {
        public BaseGenerationContext(
            Guid subjectId,
            Guid subjectVisitId,
            string visitId,
            string siteId,
            IRTEvent irtEvent,
            Guid userId,
            VisitContext visitContext = null)
        {
            if (string.IsNullOrWhiteSpace(visitId))
            {
                throw new ArgumentException($"'{nameof(visitId)}' cannot be null or whitespace.", nameof(visitId));
            }

            if (string.IsNullOrWhiteSpace(siteId))
            {
                throw new ArgumentException($"'{nameof(siteId)}' cannot be null or whitespace.", nameof(siteId));
            }

            SubjectId = subjectId;
            SubjectVisitId = subjectVisitId;
            VisitId = visitId;
            SiteId = siteId;
            IRTEvent = irtEvent ?? throw new ArgumentNullException(nameof(irtEvent));
            UserId = userId;
            VisitContext = visitContext;
        }

        public virtual Guid SubjectId { get; }

        public virtual Guid SubjectVisitId { get; }

        public virtual string VisitId { get; }

        public virtual string SiteId { get; }

        public virtual IRTEvent IRTEvent { get; }

        public virtual Guid UserId { get; }

        public virtual VisitContext VisitContext { get; }
    }
}
