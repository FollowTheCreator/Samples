using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Domain.ViewsSql.Subject;
using IRT.Domain.ViewsSql.SubjectOperationLog;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.GenericDataGeneration.Events;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.StudyEvent;
using IRT.Modules.DataTransfer.Generic.Edc.Helpers.RepeatKey;
using IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.Models;
using Kernel.DDD.Domain.Events;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices
{
    [Description("RaveRandomizationNotificationDataService")]
    public class RaveRandomizationNotificationDataService : BaseGenericNotificationDataService<RaveRandomizationViewModel>
    {
        private readonly IQueryable<SubjectVisitSqlView> subjectVisitQuery;
        private readonly IQueryable<SubjectVisitCapturedDataSqlView> subjectVisitCapturedDataQuery;

        public RaveRandomizationNotificationDataService(
            IRTDbContext context,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IOptions<IrtAppConfigManager> appSettings,
            IQueryable<GenericStudyEventRepeatKeySqlView> genericStudyEventRepeatKeysQuery,
            IQueryable<GenericItemGroupRepeatKeySqlView> genericItemGroupRepeatKeysQuery,
            StudyEventRepeatKeyHelper studyEventRepeatKeyHelper,
            ItemGroupRepeatKeyHelper itemGroupRepeatKeyHelper,
            IQueryable<SubjectVisitSqlView> subjectVisitQuery,
            IQueryable<SubjectVisitCapturedDataSqlView> subjectVisitCapturedDataQuery)
            : base(
                context,
                extendedPropertiesValueProvider,
                appSettings,
                genericStudyEventRepeatKeysQuery,
                genericItemGroupRepeatKeysQuery,
                studyEventRepeatKeyHelper,
                itemGroupRepeatKeyHelper)
        {
            this.subjectVisitQuery = subjectVisitQuery;
            this.subjectVisitCapturedDataQuery = subjectVisitCapturedDataQuery;
        }

        public override void MapModelDataInternal(NotificationSqlView notification, Event e)
        {
            SetRepeatKeys(notification);
            SetRandomizationVisitCapturedData(notification);
        }

        private void SetRandomizationVisitCapturedData(NotificationSqlView notification)
        {
            var visitData = subjectVisitCapturedDataQuery
                    .Where(x => x.SubjectVisitId == notification.SubjectVisitId)
                    .ToList();

            // Set Captured Data
            ModelData.SubjectVisitCapturedDataQuery = visitData;

            if (ModelData.IsBackout)
            {
                ModelData.RandomizationNumberId = string.Empty;
                ModelData.RandomizationDate = string.Empty;
                return;
            }

            // Set Randomization Number
            ModelData.RandomizationNumberId = visitData.FirstOrDefault(x => x.FieldName == "RandomizationNumberId")?.Value;

            var dateValue = visitData.FirstOrDefault(x => x.FieldName == "EnrollmentDate")?.Value;

            if (DateTime.TryParse(dateValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate)) 
            {
                // Set Randomization Date
                ModelData.RandomizationDate = parsedDate.ToString("dd MMM yyyy");
            }
        }
    }
}
