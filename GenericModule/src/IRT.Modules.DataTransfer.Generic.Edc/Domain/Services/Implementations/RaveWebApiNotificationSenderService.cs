using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications;
using Frameworks.Notifications.Commands;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.Notifications;
using IRT.Domain.Services.Impl;
using IRT.Domain.ViewsSql.Study;
using IRT.Modules.DataTransfer.Generic.Domain.Configuration;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces.Clients;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.ApacheHop;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Aggregates.ClientNotifications.Commands.Rave;
using Kernel.AspNetMvc.DynamicData.Services.Interfaces;
using Kernel.Globalization.Constants;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Services.Implementations
{
    public class RaveWebApiNotificationSenderService : WebApiNotificationSenderService, IRaveWebApiNotificationSenderService
    {
        private const string NotificationId = "$.NotificationId";
        private const string ApacheHopUrlFieldName = "ApacheHopUrl";
        private const string GenericIntegrationFields = "GenericIntegrationFields";

        protected readonly IExtendedPropertiesValueProvider ExtendedPropertiesValueProvider;
        protected readonly IOptions<DTGenericConfigManager> ConfigManager;
        protected readonly IRTDbContext IrtDbContext;

        private readonly IDynamicFormFieldRegistry dynamicFormFieldRegistry;

        public bool IsFailedNotificationsSenderJob { get; set; }

        public int MaxNumberOfRetries { get; set; }

        public RaveWebApiNotificationSenderService(
            Kernel.DDD.Dispatching.ICommandBus commandBus,
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IOptions<DTGenericConfigManager> configManager,
            IRTDbContext irtDbContext,
            IDynamicFormFieldRegistry dynamicFormFieldRegistry)
            : base(commandBus)
        {
            ExtendedPropertiesValueProvider = extendedPropertiesValueProvider;
            ConfigManager = configManager;
            IrtDbContext = irtDbContext;
            this.dynamicFormFieldRegistry = dynamicFormFieldRegistry;

            HttpClientInitializing += WebApiNotificationSenderService_HttpClientInitializing;
        }

        public override IQueryable<NotificationSqlView> FilterPendingNotifications(IQueryable<NotificationSqlView> pendingNotificationsQuery)
        {
            return !IsFailedNotificationsSenderJob && MaxNumberOfRetries > 0 ?
                base.FilterPendingNotifications(pendingNotificationsQuery)
                    .Where(x => !x.FailedSendAttempts.HasValue || x.FailedSendAttempts < MaxNumberOfRetries) :
                base.FilterPendingNotifications(pendingNotificationsQuery);
        }

        protected override void IncrementFailedSendAttempts(NotificationSqlView notification)
        {
            var localizedContent = notification.NotificationLocalizedContentEntries.FirstOrDefault(x => x.LanguageId == GlobalizationConstants.DefaultLanguage);
            if (localizedContent.AttachmentName != null)
            {
                Logger.Error("Generic integration failed for {0} ({1}) with the response: {2}".F(
                    localizedContent.Title,
                    notification.Id.ToString(),
                    Encoding.UTF8.GetString(localizedContent.Attachment)));
                var result = CommandBus.SendCommand(new IncrementNotificationFailedSendAttemptsWithAttachment
                {
                    NotificationId = notification.Id,
                    AttachmentName = localizedContent.AttachmentName,
                    AttachmentMediaType = notification.AttachmentMediaType,
                    AttachmentContent = localizedContent.Attachment
                });
                if (result.IsFailure)
                {
                    throw result.Exception;
                }
            }
            else
            {
                Logger.Error("Generic integration failed for {0} ({1})".F(localizedContent.Title, notification.Id.ToString()));
                base.IncrementFailedSendAttempts(notification);
            }
        }

        protected override void MarkNotificationAsSent(NotificationSqlView notification)
        {
            var localizedContent = notification.NotificationLocalizedContentEntries.FirstOrDefault(x => x.LanguageId == GlobalizationConstants.DefaultLanguage);
            if (localizedContent.AttachmentName != null)
            {
                var result = CommandBus.SendCommand(new MarkNotificationAsSentWithAttachment
                {
                    NotificationId = notification.Id,
                    AdditionalInfo = notification.AdditionalInfo,
                    AttachmentName = localizedContent.AttachmentName,
                    AttachmentMediaType = notification.AttachmentMediaType,
                    AttachmentContent = localizedContent.Attachment
                });
                if (result.IsFailure)
                {
                    throw result.Exception;
                }
            }
            else
            {
                base.MarkNotificationAsSent(notification);
            }
        }

        protected override bool SendNotificationInternal(NotificationSqlView notification)
        {
            var notificationDefinitionIds = NotificationDefinitionRegistry.GetNotificationDefinitions(null)
              .Where(x => x.PendingNotificationSenderTypes.Contains(typeof(RaveWebApiNotificationSenderService)))
              .Select(x => x.Id)
              .ToList();

            if (notificationDefinitionIds.Contains(notification.NotificationDefinitionId))
            {
                return SendClientNotificationInternal(notification);
            }

            return false;
        }

        protected override NotificationSendingResult RetransmitNotificationInternal(NotificationSqlView notification, Guid userId)
        {
            throw new NotImplementedException();
        }

        private void WebApiNotificationSenderService_HttpClientInitializing(object sender, HttpClientEventArgs e)
        {
            var settings = ExtendedPropertiesValueProvider.GetValue<StudySqlView, GenericDataTransferStudySettings>(null);
            if (settings.Timeout > 0)
            {
                e.HttpClient.Timeout = new TimeSpan(0, 0, settings.Timeout.Value);
            }

            e.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                scheme: "Basic",
                parameter: Convert.ToBase64String(
                    inArray: Encoding.ASCII.GetBytes(
                        s: "{0}:{1}"
                            .F(ConfigManager.Value.ApacheHopUsername, ConfigManager.Value.ApacheHopPassword))));
        }

        protected bool SendClientNotificationInternal(NotificationSqlView notification)
        {
            var notificationBody = notification.NotificationLocalizedContentEntries != null &&
                                   notification.NotificationLocalizedContentEntries.Any(x =>
                                       x.LanguageId.IsNullOrEmpty() ||
                                       x.LanguageId == GlobalizationConstants.DefaultLanguage)
                ? notification.NotificationLocalizedContentEntries.FirstOrDefault(x =>
                    x.LanguageId.IsNullOrEmpty() || x.LanguageId == GlobalizationConstants.DefaultLanguage).Body
                : null;

            var clientNotificationId = notification.Model != null ?
                GetClientNotificationIdForClientNotification(notification) :
                Guid.Empty.ToString();

            var requestContent = JsonConvert.SerializeObject(new ApacheHopClientNotificationRequestModel
            {
                ClientNotificationId = clientNotificationId,
                ClientPayload = notificationBody
            });

            var dynamicFields = dynamicFormFieldRegistry.GetDynamicFieldsForForm(GenericIntegrationFields);
            var dynamicDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(notification.NotificationDefinition.DynamicDataJson ?? "{}");
            var apacheHopUrlDynamicField = dynamicFields
                .SingleOrDefault(x => x.FieldName.Equals(ApacheHopUrlFieldName, StringComparison.OrdinalIgnoreCase));

            var serviceName = dynamicDataDictionary.TryGetValue(apacheHopUrlDynamicField.FieldName, out var value)
                ? value
                : null;

            var requestHttpContent = new StringContent(requestContent, Encoding.UTF8, "application/json");

            var response = HttpPost(ConfigManager.Value.ApacheHopBaseUrl + "/hop/webService/?service=" + serviceName, requestHttpContent);

            return ProcessClientNotificationResult(response, notification.Id);
        }

        private bool ProcessClientNotificationResult(HttpResponseMessage response, Guid clientNotificationId)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            string responseBody = response.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(responseBody))
            {
                return false;
            }

            var result = JsonConvert.DeserializeObject<ApacheHopClientNotificationResponseModel>(responseBody);
            if (result == null || result.ExecutionResult == null)
            {
                return false;
            }

            var executionResult = result.ExecutionResult[0];
            var isSuccess = executionResult.IsSuccess;
            CommandBus.SendCommand(new RaveAknowledgeClientNotification
            {
                ClientNotificationId = clientNotificationId,
                IsSuccess = isSuccess,
                ErrorCode = executionResult.ErrorCode,
                ResponseMessage = executionResult.ResponseMessage,
                Url = executionResult.Url,
                FileOid = executionResult.FileOid
            });

            return isSuccess;
        }

        private string GetClientNotificationIdForClientNotification(NotificationSqlView clientNotification)
        {
            JObject jObj = JObject.Parse(clientNotification.Model);
            return jObj
                .SelectToken(NotificationId)
                ?.ToString();
        }
    }
}