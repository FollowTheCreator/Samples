using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.Aggregates.SelfSupportWorkflowOrchestrator.ValueObjects;
using IRT.Domain.ViewsSql.Subject;
using IRT.Domain.ViewsSql.Subject.SubjectVisitDrug;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.GenericDataGeneration.Events;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Models;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.StudyEvent;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;
using IRT.Modules.DataTransfer.Generic.Edc.Helpers.RepeatKey;
using Kernel.DDD.Domain.Events;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices
{
    [Description("GenericDrugDispensationNotificationDataProvider")]
    public class GenericDrugDispensationNotificationDataService : BaseGenericNotificationDataService<GenericDrugDispensationViewModel>
    {
        private readonly SerializableItemGroupRepeatKeyHelper itemGroupRepeatKeySerializableHelper;

        private readonly IQueryable<SubjectVisitSqlView> subjectVisitQuery;

        public GenericDrugDispensationNotificationDataService(
            SerializableItemGroupRepeatKeyHelper itemGroupRepeatKeySerializableHelper,
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
            this.itemGroupRepeatKeySerializableHelper = itemGroupRepeatKeySerializableHelper;
            this.subjectVisitQuery = subjectVisitQuery;
        }

        public override void MapModelDataInternal(NotificationSqlView notification, Event e)
        {
            string jsonSubjectVisitSqlView = GetJsonSubjectVisitSqlView(notification.SubjectVisitId);
            ModelData.JsonSubjectVisitSqlView = jsonSubjectVisitSqlView;

            string jsonSubjectVisitLabeledDrugSqlView = GetSubjectVisitLabeledDrugSqlView(
                notification,
                e);
            ModelData.JsonSubjectVisitLabeledDrugSqlView = jsonSubjectVisitLabeledDrugSqlView;

            SetFormRepeatKeys();
            SetStudyEventRepeatKeys(notification);
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

        private string GetSubjectVisitLabeledDrugSqlView(
            NotificationSqlView notification,
            Event e)
        {
            if (e is GenericIntegrationDataEvent genericEvent)
            {
                if (genericEvent.SelfSupportModificationRequest != null) // SelfSupportChangeRequestProcessed event
                {
                    var request = genericEvent.SelfSupportModificationRequest;

                    switch (request.DataChangeType)
                    {
                        case SubjectSelfSupportDataChangeType.UpdateAssignedDrugs:
                            var selfSupportDrugsToSerialize = itemGroupRepeatKeySerializableHelper.GetSelfSupportSerializableDrugUnits(
                                request,
                                GenericNotificationDefinitionSettings,
                                notification);

                            if (selfSupportDrugsToSerialize != null)
                            {
                                return SerializeDrugs(selfSupportDrugsToSerialize);
                            }

                            break;

                        case SubjectSelfSupportDataChangeType.BackOutTransaction:
                            var backoutDrugsToSerialize = itemGroupRepeatKeySerializableHelper.GetBackoutSerializableDrugUnits(
                                request,
                                GenericNotificationDefinitionSettings,
                                notification);

                            return SerializeDrugs(backoutDrugsToSerialize);
                    }
                }

                var drugs = itemGroupRepeatKeySerializableHelper.GetSerializableDrugUnits(notification);

                return SerializeDrugs(drugs);
            }

            return "null";
        }

        private string SerializeDrugs(List<SerializableDrugUnit> drugs)
        {
            if (drugs != null && drugs.Any())
            {
                drugs
                    .ForEach(x =>
                    {
                        var assignedRepeatKey = FormatRepeatKey(x.AssignedItemGroupRepeatKey.RepeatKey);

                        x.AssignedItemGroupRepeatKey.RepeatKey = assignedRepeatKey;

                        if (x.ReplacedItemGroupRepeatKey?.RepeatKey != null)
                        {
                            var replacedRepeatKey = FormatRepeatKey(x.ReplacedItemGroupRepeatKey.RepeatKey);

                            x.ReplacedItemGroupRepeatKey.RepeatKey = replacedRepeatKey;
                        }
                    });

                return JsonSerializer.Serialize(drugs);
            }
            else
            {
                return "null";
            }
        }

        private string FormatRepeatKey(string repeatKey)
        {
            var repeatKeyFormat = ItemGroupRepeatKeyHelper.GetItemGroupRepeatKeyFormat(
                repeatKey,
                GenericNotificationDefinitionSettings,
                GenericVisitSettings);

            var formattedItemGroupRepeatKey = repeatKeyFormat
                ?.F(repeatKey)
                ?? null;

            return formattedItemGroupRepeatKey;
        }
    }
}
