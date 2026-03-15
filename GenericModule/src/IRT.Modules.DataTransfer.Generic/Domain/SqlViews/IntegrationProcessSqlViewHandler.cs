using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kendo.Mvc.Extensions;
using NLog;
using Frameworks.Notifications;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.Constants;
using IRT.Domain.Notifications;
using IRT.Domain.ViewsSql.Notifications;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.GenericDataGeneration.Events;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDefinition;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDependency;
using Kernel.DDD.Dispatching;
using Kernel.DDD.Domain.Events;
using Kernel.Globalization.Utilities;
using Unity;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews
{
    [Priority(EventHandlerPriorityStages.NotificationsSubsystem)]
    public class IntegrationProcessSqlViewHandler : NotificationSqlViewHandlerBase
    {
        private readonly IEventGenerationService eventGenerationService;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly IGenericNotificationDependencyService genericNotificationDependencyService;
        protected readonly IGenericNotificationDefinitionService genericNotificationDefinitionService;

        public IntegrationProcessSqlViewHandler(
            IEventGenerationService eventGenerationService,
            IGenericNotificationDependencyService genericNotificationDependencyService,
            IGenericNotificationDefinitionService genericNotificationDefinitionService)
        {
            this.eventGenerationService = eventGenerationService;
            this.genericNotificationDependencyService = genericNotificationDependencyService;
            this.genericNotificationDefinitionService = genericNotificationDefinitionService;
        }

        protected virtual void HandleInternal(IRTEvent irtEvent)
        {
            try
            {
                IEnumerable<GenericIntegrationDataEvent> generatedEvents = null;
                CultureHelper.ExecuteInCulture(() => generatedEvents = eventGenerationService.CreateGenericDataEvent(irtEvent));

                foreach (var e in generatedEvents)
                {
                    var notificationDefinition = NotificationDefinitionRegistry.GetDefinition(e.NotificationDefinitionID);

                    AddNotification(
                        notificationDefinition,
                        e,
                        setAdditionalPropertiesAction: x =>
                        {
                            x.SiteId = e.SiteId;
                            x.SubjectId = e.SubjectId;
                            x.SubjectVisitId = e.SubjectVisitId;
                            x.VisitId = e.VisitId != null
                                ? e.VisitId
                                : e.VisitContext != null
                                    ? e.VisitContext.VisitId
                                    : null;
                            x.AdditionalInfo = e.AdditionalInfo;
                        });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        // ToDo: This should be part of the core
        #region Add Razor Notification
        [Dependency]
        public IEnumerable<INotificationDefinitionRenderer> NotificationRazorDefinitionRenderers { get; set; }

        public bool IsEnabled(Guid notificationDefinitionId) => NotificationDefinitionRegistry.GetEnabledNotificationDefinitions(null).Any(x => x.Id == notificationDefinitionId);

        protected override void AddNotification(
            NotificationDefinition definition,
            IRTEvent e,
            Action<NotificationSqlView> setAdditionalPropertiesAction = null,
            bool immediatelyCallSaveChanges = true)
        {
            var notification = new NotificationSqlView(
                definition: definition,
                generatedLocalDate: e.TransactionLocalDateTime,
                generatedUtcDateTime: e.TransactionUtcDateTime)
            {
                OriginatorUserId = new Guid(e.CommandMetadata.UserId)
            };

            if (setAdditionalPropertiesAction != null)
            {
                setAdditionalPropertiesAction(notification);
            }

            RenderAndSaveRazorNotification(
                notification: notification,
                notificaitonDefinition: definition,
                e: e,
                immediatelyCallSaveChanges: immediatelyCallSaveChanges,
                culture: null);

            EnqueueNotification(notification);
        }

        private void RenderAndSaveRazorNotification(
            NotificationSqlView notification,
            NotificationDefinition notificaitonDefinition,
            Event e,
            bool immediatelyCallSaveChanges = true,
            CultureInfo culture = null)
        {
            var renderInstance = NotificationRazorDefinitionRenderers.Where(x => x.GetType() == notificaitonDefinition.RendererType).FirstOrDefault();

            renderInstance.RenderAndSetModel(
                    notification: notification,
                    e: e,
                    culture: culture);

            Db.Notifications.Add(notification);

            ResourceKeysDisplayHelper.ExecuteIfShouldDisplayResourceKey(() =>
            {
                var notificationIds = notification.NotificationLocalizedContentEntries
                    .Select(x => x.Id);

                MarkNotificationsWithResourceKeys(notificationIds);
            });

            if (immediatelyCallSaveChanges)
            {
                Db.SaveChanges();
            }

            return;
        }

        private void MarkNotificationsWithResourceKeys(IEnumerable<Guid> notificationLocalizedContentIds)
        {
            var items = notificationLocalizedContentIds
                .Select(x => new NotificationLocalizedContentWithResourceKeysSqlView()
                {
                    NotificationLocalizedContentId = x
                });

            Db.NotificationLocalizedContentWithResourceKeys.AddRange(items);
        }

        private void EnqueueNotification(NotificationSqlView notification)
        {
            if (!genericNotificationDefinitionService.GetIdsFor(NotificationVendor.Veeva).Contains(notification.NotificationDefinitionId))
            {
                return;
            }

            var groupKey = notification.SubjectId.ToString();

            genericNotificationDependencyService.EnqueueNotification(groupKey, notification.Id);

            Db.SaveChanges();
        }
        #endregion
    }
}