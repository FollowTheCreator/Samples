using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.Aggregates.SelfSupportWorkflowOrchestrator.ValueObjects;
using IRT.Domain.Notifications;
using IRT.Domain.ViewsSql.Notifications;
using IRT.Domain.ViewsSql.SelfSupportWorkflows;
using IRT.Domain.ViewsSql.Study;
using IRT.Domain.ViewsSql.SubjectOperationLog;
using IRT.Domain.ViewsSql.Visit;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.GenericDataGeneration.Events;
using IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices.Interfaces;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices.Models;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.StudyEvent;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.RepeatKey;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Edc.Helpers.RepeatKey;
using Kernel.DDD.Domain.Events;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Notifications.DataServices
{
    public abstract class BaseGenericNotificationDataService
        <T> : SubjectDataServiceBase<T>, IGenericNotificationDataService
        where T : GenericBaseSubjectViewModel, new()
    {
        protected readonly IQueryable<GenericStudyEventRepeatKeySqlView> GenericStudyEventRepeatKeysQuery;
        protected readonly IQueryable<GenericItemGroupRepeatKeySqlView> GenericItemGroupRepeatKeysQuery;
        protected readonly StudyEventRepeatKeyHelper StudyEventRepeatKeyHelper;
        protected readonly ItemGroupRepeatKeyHelper ItemGroupRepeatKeyHelper;

        public GenericEdcDataTransferStudySettings GenericDataTransferStudySettings { get; set; }

        public GenericEdcNotificationDefinitionSettings GenericNotificationDefinitionSettings { get; set; }

        public GenericEdcVisitSettings GenericVisitSettings { get; set; }

        public BaseGenericNotificationDataService(
            IRTDbContext context,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IOptions<IrtAppConfigManager> appSettings,
            IQueryable<GenericStudyEventRepeatKeySqlView> genericStudyEventRepeatKeysQuery,
            IQueryable<GenericItemGroupRepeatKeySqlView> genericItemGroupRepeatKeysQuery,
            StudyEventRepeatKeyHelper studyEventRepeatKeyHelper,
            ItemGroupRepeatKeyHelper itemGroupRepeatKeyHelper)
            : base(
                context,
                extendedPropertiesValueProvider,
                appSettings)
        {
            GenericDataTransferStudySettings = extendedPropertiesValueProvider.GetValue<StudySqlView, GenericEdcDataTransferStudySettings>(null);
            GenericStudyEventRepeatKeysQuery = genericStudyEventRepeatKeysQuery;
            GenericItemGroupRepeatKeysQuery = genericItemGroupRepeatKeysQuery;
            StudyEventRepeatKeyHelper = studyEventRepeatKeyHelper;
            ItemGroupRepeatKeyHelper = itemGroupRepeatKeyHelper;
        }

        public abstract void MapModelDataInternal(NotificationSqlView notification, Event e);

        protected override void SetupDataService(NotificationSqlView notification)
        {
            base.SetupDataService(notification);
        }

        public override void InitializeDataService(NotificationSqlView notification, Event e)
        {
            base.InitializeDataService(notification, e);
        }

        public override void PersistModelData(NotificationSqlView notification)
        {
            base.PersistModelData(notification);
        }

        protected override void MapModelData(NotificationSqlView notification, Event e)
        {
            base.MapModelData(notification, e);

            PreProcessNotification(notification, e);

            ModelData.NotificationId = notification.Id;

            ModelData.CreationDateTime = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");

            string jsonSubject = JsonSerializer.Serialize(ModelData.Subject);
            ModelData.JsonSubject = jsonSubject;

            string jsonSite = JsonSerializer.Serialize(ModelData.Site);
            ModelData.JsonSite = jsonSite;

            string jsonDemographicCountrySettings = JsonSerializer.Serialize(ModelData.DemographicCountrySettings);
            ModelData.JsonDemographicCountrySettings = jsonDemographicCountrySettings;

            string jsonNotification = JsonSerializer.Serialize(ModelData.Notification);
            ModelData.JsonNotification = jsonNotification;

            string jsonStudy = JsonSerializer.Serialize(ModelData.Study);
            ModelData.JsonStudy = jsonStudy;

            string jsonStudySettings = JsonSerializer.Serialize(ModelData.StudySettings);
            ModelData.JsonStudySettings = jsonStudySettings;

            string jsonStudyLimits = JsonSerializer.Serialize(ModelData.StudyLimits);
            ModelData.JsonStudyLimits = jsonStudyLimits;

            MapModelDataInternal(notification, e);

            PostProcessNotification(notification, e);
        }

        public void PreProcessNotification(NotificationSqlView notification, Event e)
        {
            ModelData.IsBackout = false;

            var notificationDefinition = NotificationDefinitionRegistry.GetDefinition(notification.NotificationDefinitionId);
            if (notificationDefinition == null)
            {
                return;
            }

            GenericNotificationDefinitionSettings = ExtendedPropertiesValueProvider
                 .GetValue<NotificationDefinitionSqlView, GenericEdcNotificationDefinitionSettings>(notificationDefinition.Id.ToString());

            var visitid = notification.VisitId;

            SelfSupportModificationRequestSqlView request = null;

            if (e is GenericIntegrationDataEvent genericEvent)
            {
                var jsonNotificationGenerationSettingEntity = JsonSerializer.Serialize(genericEvent.NotificationGenerationSettingEntity);
                ModelData.JsonNotificationGenerationSettingEntity = jsonNotificationGenerationSettingEntity;

                if (genericEvent.SelfSupportModificationRequest != null) // SelfSupportChangeRequestProcessed event
                {
                    request = genericEvent.SelfSupportModificationRequest;

                    switch (request.DataChangeType)
                    {
                        case SubjectSelfSupportDataChangeType.BackOutTransaction:
                            ModelData.IsBackout = true;
                            visitid = request.Details
                                .FirstOrDefault(x => x.ChangeKind == SubjectOperationLogChangeKind.VisitId).OldValue;

                            break;
                    }
                }
            }

            var serializeOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var jsonSelfSupportModificationRequest = JsonSerializer.Serialize(request, serializeOptions);
            ModelData.JsonSelfSupportModificationRequest = jsonSelfSupportModificationRequest;

            if (!visitid.IsNullOrEmpty())
            {
                GenericVisitSettings = ExtendedPropertiesValueProvider.GetValue<VisitSqlView, GenericEdcVisitSettings>(visitid);
            }
        }

        public void PostProcessNotification(NotificationSqlView notification, Event e)
        {
            if (e is GenericIntegrationDataEvent genericEvent)
            {
                if (genericEvent.SelfSupportModificationRequest != null) // SelfSupportChangeRequestProcessed event
                {
                    // we need to clear this information to hide the notification from the core IRT backout functionality that will delete all applicable notifications
                    if (genericEvent.SubjectBackout)
                    {
                        notification.SubjectId = null;
                    }

                    if (genericEvent.VisitBackout)
                    {
                        notification.SubjectVisitId = null;
                    }
                }
            }
        }

        public void SetRepeatKeys(NotificationSqlView notification)
        {
            SetFormRepeatKeys();
            SetStudyEventRepeatKeys(notification);
            SetItemGroupRepeatKeys(notification);
        }

        public void SetFormRepeatKeys()
        {
            var formRepeatKey = GenericNotificationDefinitionSettings.FormRepeatKeysEnabled
                ? GenericVisitSettings?.FormRepeatKeyFormat
                    ?? GenericNotificationDefinitionSettings.FormRepeatKeyFormat
                    ?? null
                : null;

            var jsonFormRepeatKey = JsonSerializer.Serialize(formRepeatKey);

            ModelData.FormRepeatKey = jsonFormRepeatKey;
        }

        public void SetStudyEventRepeatKeys(NotificationSqlView notification)
        {
            string studyEventRepeatKey = null;

            if (GenericNotificationDefinitionSettings.StudyEventRepeatKeysEnabled)
            {
                var studyEventRepeatKeyInfo = GenericStudyEventRepeatKeysQuery
                    .FirstOrDefault(x => x.SubjectVisitId == notification.SubjectVisitId
                        && x.NotificationDefinitionId == notification.NotificationDefinitionId);

                if (studyEventRepeatKeyInfo != null)
                {
                    var repeatKeyFormat = StudyEventRepeatKeyHelper.GetStudyEventRepeatKeyFormat(
                        studyEventRepeatKeyInfo.RepeatKey,
                        GenericNotificationDefinitionSettings,
                        GenericVisitSettings);

                    studyEventRepeatKey = repeatKeyFormat
                        ?.F(
                            studyEventRepeatKeyInfo.RepeatKey,
                            studyEventRepeatKeyInfo.ScheduledRepeatKey,
                            studyEventRepeatKeyInfo.UnscheduledRepeatKey,
                            studyEventRepeatKeyInfo.ReplacementRepeatKey,
                            studyEventRepeatKeyInfo.ScreenFailRepeatKey,
                            studyEventRepeatKeyInfo.InformedConsentRepeatKey)
                        ?? null;
                }
            }

            var jsonStudyEventRepeatKey = JsonSerializer.Serialize(studyEventRepeatKey);

            ModelData.StudyEventRepeatKey = jsonStudyEventRepeatKey;
        }

        public void SetItemGroupRepeatKeys(NotificationSqlView notification)
        {
            SerializableRepeatKey itemGroupRepeatKey = null;

            if (GenericNotificationDefinitionSettings.ItemGroupRepeatKeysEnabled)
            {
                var itemGroupRepeatKeyInfo = GenericItemGroupRepeatKeysQuery
                    .FirstOrDefault(x => x.SubjectVisitId == notification.SubjectVisitId
                        && x.NotificationDefinitionId == notification.NotificationDefinitionId);

                if (itemGroupRepeatKeyInfo != null)
                {
                    var repeatKeyFormat = ItemGroupRepeatKeyHelper.GetItemGroupRepeatKeyFormat(
                        itemGroupRepeatKeyInfo.RepeatKey,
                        GenericNotificationDefinitionSettings,
                        GenericVisitSettings);

                    var repeatKey = repeatKeyFormat
                        ?.F(itemGroupRepeatKeyInfo.RepeatKey)
                        ?? null;

                    var repeatKeyTransactionType = itemGroupRepeatKeyInfo.TransactionType;

                    itemGroupRepeatKey = new SerializableRepeatKey
                    {
                        RepeatKey = repeatKey,
                        TransactionType = repeatKeyTransactionType
                    };
                }
            }

            ModelData.ItemGroupRepeatKey = itemGroupRepeatKey;
        }

        public void SetVisitMapping(NotificationSqlView notification)
        {
            var edcVisitId = GenericVisitSettings.VisitMapping ?? notification.VisitId;

            ModelData.VisitEdcMapping = edcVisitId;
        }
    }
}
