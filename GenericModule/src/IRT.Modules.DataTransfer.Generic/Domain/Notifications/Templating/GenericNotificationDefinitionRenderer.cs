using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NLog;
using RazorEngine;
using RazorEngine.Templating;
using Frameworks.Notifications;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.Notifications;
using IRT.Domain.Notifications.Models;
using Kernel.DDD.Domain.Events;
using Kernel.Globalization.Constants;
using Kernel.Globalization.Utilities;
using Kernel.Utilities;
using Kernel.Utilities.Extensions;
using Unity;

namespace IRT.Modules.DataTransfer.Generic.Domain.Notifications.Templating
{
    public class GenericNotificationDefinitionRenderer : NotificationDefinitionRenderer
    {
        private readonly IRTDbContext irtDbContext;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public GenericNotificationDefinitionRenderer(
            IUnityContainer unityContainer,
            IRTDbContext irtDbContext)
            : base(unityContainer)
        {
            this.irtDbContext = irtDbContext;
        }

        public override void RenderAndSetModel(NotificationSqlView notification, Event e, CultureInfo culture = null)
        {
            var definition = NotificationDefinitionRegistry.GetDefinition(notification.NotificationDefinitionId);
            var dataService = (INotificationDataServiceBase<object>)unityContainer.Resolve(definition.DataServiceType);

            dataService.InitializeDataService(
                    notification: notification,
                    e: e);

            dataService.InitializeExtensions(
                notification: notification,
                e: e,
                unityContainer: unityContainer);

            dataService.PersistModelData(notification);

            SetNotificationsContent(
                notification: notification,
                dataService: dataService,
                e: e);
        }

        public override void RenderBasedOnModel(NotificationSqlView notification, Event e = null, CultureInfo culture = null)
        {
            throw new NotImplementedException();
        }

        public override string RenderTitle(NotificationSqlView notification, CultureInfo culture = null)
        {
            throw new NotImplementedException();
        }

        protected virtual void SetNotificationsContent(
            NotificationSqlView notification,
            INotificationDataServiceBase<object> dataService,
            Event e = null)
        {
            var renderInCultures = new List<CultureInfo>
            {
                new CultureInfo(GlobalizationConstants.DefaultLanguage)
            };

            notification.NotificationLocalizedContentEntries ??= new List<NotificationLocalizedContentSqlView>();

            foreach (var renderCulture in renderInCultures)
            {
                var content = RenderNotificationLocalizedContent(
                    notification: notification,
                    dataService: dataService,
                    culture: renderCulture);

                notification.NotificationLocalizedContentEntries.Add(new NotificationLocalizedContentSqlView()
                {
                    Id = SequenticalGuid.NewGuid(),
                    NotificationId = notification.Id,
                    LanguageId = renderCulture.Name,
                    Title = content.Title,
                    Body = content.Body,
                    AttachmentName = content.AttachmentName,
                    Attachment = content.Attachment,
                    GeneratedUtcDateTime = e != null
                        ? e.EventMetadata.OccurredDateTimeUtc
                        : notification.GeneratedUtcDateTime,
                });
            }
        }

        private NotificationLocalizedContent RenderNotificationLocalizedContent(
             NotificationSqlView notification,
             INotificationDataServiceBase<object> dataService,
             CultureInfo culture)
        {
            var languageId = culture.Name;

            var notificationContent = new NotificationLocalizedContent()
            {
                NotificationId = notification.Id,
                LanguageId = languageId
            };

            CultureHelper.ExecuteInCulture(action: () =>
            {
                notificationContent.GeneratedUtcDateTime = notification.GeneratedUtcDateTime;

                Logger.Trace("Running title and body templates, [{0}].".F(languageId));

                var notificationDefinition = irtDbContext.NotificationDefinitions
                    .Include(n => n.NotificationTemplate)
                    .SingleOrDefault(n => n.NotificationTypeId == notification.NotificationDefinitionId);

                notificationContent.Title = Run(notificationDefinition.NotificationTemplate.Title, "Title-" + notification.NotificationDefinitionId, dataService);
                notificationContent.Body = Run(notificationDefinition.NotificationTemplate.Body, "Body-" + notification.NotificationDefinitionId, dataService);
            }, culture);

            return notificationContent;
        }

        private string Run(string template, string templateName, object model)
        {
            var templateKey = Engine.Razor.GetKey(templateName);

            if (!Engine.Razor.IsTemplateCached(templateKey, model.GetType()))
            {
                try
                {
                    Engine.Razor.Compile(template, templateName, model.GetType());
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }

            return Engine.Razor.Run(templateName, model.GetType(), model);
        }
    }
}
