using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.ViewsSql.Subject;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.StudyEvent;
using IRT.Modules.DataTransfer.Generic.Edc.Helpers.RepeatKey;
using IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices.Models;
using Kernel.DDD.Domain.Events;

namespace IRT.Plugins.DataTransfer.Generic.EdcPlugins.DataServices
{
    [Description("RaveEnrollmentNotificationDataService")]
    public class RaveEnrollmentNotificationDataService : BaseGenericNotificationDataService<RaveEnrollmentViewModel>
    {
        private readonly IQueryable<SubjectVisitSqlView> subjectVisitQuery;

        public RaveEnrollmentNotificationDataService(
            IRTDbContext context,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IOptions<IrtAppConfigManager> appSettings,
            IQueryable<GenericStudyEventRepeatKeySqlView> genericStudyEventRepeatKeysQuery,
            IQueryable<GenericItemGroupRepeatKeySqlView> genericItemGroupRepeatKeysQuery,
            StudyEventRepeatKeyHelper studyEventRepeatKeyHelper,
            ItemGroupRepeatKeyHelper itemGroupRepeatKeyHelper,
            IQueryable<SubjectVisitSqlView> subjectVisitQuery)
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
        }

        public override void MapModelDataInternal(NotificationSqlView notification, Event e)
        {
            string jsonSubjectVisitSqlView = GetJsonSubjectVisitSqlView(notification.SubjectVisitId);
            ModelData.JsonSubjectVisitSqlView = jsonSubjectVisitSqlView;

            SetRepeatKeys(notification);
        }

        private string GetJsonSubjectVisitSqlView(Guid? subjectVisitId)
        {
            var subjectVisit = subjectVisitQuery
                .Where(x => x.Id == subjectVisitId)
                .FirstOrDefault();

            if (subjectVisit != null)
            {
                return JsonSerializer.Serialize(subjectVisit);
            }
            else
            {
                return "null";
            }
        }
    }
}
