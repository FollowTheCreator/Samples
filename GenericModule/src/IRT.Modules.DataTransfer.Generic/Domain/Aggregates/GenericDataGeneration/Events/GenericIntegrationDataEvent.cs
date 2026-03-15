using System;
using System.Collections.Generic;
using Frameworks.ExtendedProperties.Dto;
using IRT.Domain;
using IRT.Domain.ViewsSql.SelfSupportWorkflows;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using Kernel.SharedDomain.ValueObjects.Visit;

namespace IRT.Modules.DataTransfer.Generic.Domain.Aggregates.GenericDataGeneration.Events
{
    public class GenericIntegrationDataEvent : IRTEvent
    {
        public Guid? NotificationId { get; set; }

        public Guid SubjectId { get; set; }

        public string SubjectKey { get; set; }

        public Guid SubjectVisitId { get; set; }

        public string VisitId { get; set; }

        public string VisitName { get; set; }

        public string SiteId { get; set; }

        public Guid UserId { get; set; }

        public Guid NotificationDefinitionID { get; set; }

        public string AdditionalInfo { get; set; }

        public SelfSupportModificationRequestSqlView SelfSupportModificationRequest { get; set; }

        public ExtendedPropertyEntityDto<NotificationGenerationSettings> NotificationGenerationSettingEntity { get; set; }

        public bool VisitBackout { get; set; }

        public bool SubjectBackout { get; set; }

        public VisitContext VisitContext { get; set; }

        public Dictionary<string, string> CapturedData { get; set; }
    }
}