using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using Frameworks.ExtendedProperties.Providers;
using Frameworks.Notifications.Entities;
using IRT.Domain;
using IRT.Domain.ViewsSql.Study;
using IRT.Modules.DataTransfer.Generic.Domain.Configuration;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings;
using Kernel.AspNetMvc.DynamicData.Services.Interfaces;
using Kernel.Globalization.Attributes;
using Kernel.Jobs.Jobs;
using Kernel.Logging.Common;
using Kernel.Utilities.Extensions;
using JobsResources = IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs.Resources;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Jobs
{
    [LocalizedCategory(JobsResources.ApacheHopEndpointsHealthCheckJob.ResourceNames.DataTransferGeneric, typeof(JobsResources.ApacheHopEndpointsHealthCheckJob))]
    [LocalizedDisplayName(JobsResources.ApacheHopEndpointsHealthCheckJob.ResourceNames.DisplayName, typeof(JobsResources.ApacheHopEndpointsHealthCheckJob))]
    [LocalizedDescription(JobsResources.ApacheHopEndpointsHealthCheckJob.ResourceNames.Description, typeof(JobsResources.ApacheHopEndpointsHealthCheckJob))]
    [Logs(Edc.Domain.Operations.GenericDataTransfer.ViewApacheHealthJobsLogs)]
    public class ApacheHopEndpointsHealthCheckJob : BaseJob
    {
        private const string ApacheHopUrlFieldName = "ApacheHopUrl";

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IExtendedPropertiesValueProvider extendedPropertiesValueProvider;
        private readonly IOptions<DTGenericConfigManager> configManager;
        private readonly GenericDataTransferStudySettings genericDataTransferStudySettings;
        private readonly IDynamicFormFieldRegistry dynamicFormFieldRegistry;
        private readonly IRTDbContext dbContext;

        public ApacheHopEndpointsHealthCheckJob(
            IExtendedPropertiesValueProvider extendedPropertiesValueProvider,
            IOptions<DTGenericConfigManager> configManager,
            IHttpClientFactory httpClientFactory,
            IDynamicFormFieldRegistry dynamicFormFieldRegistry,
            IRTDbContext dbContext)
            : base()
        {
            this.extendedPropertiesValueProvider = extendedPropertiesValueProvider;
            this.configManager = configManager;
            this.httpClientFactory = httpClientFactory;
            this.dynamicFormFieldRegistry = dynamicFormFieldRegistry;
            this.dbContext = dbContext;
            genericDataTransferStudySettings = this.extendedPropertiesValueProvider.GetValue<StudySqlView, GenericDataTransferStudySettings>(null);
        }

        public override Task ExecuteInternal(IJobExecutionContext context)
        {
            var notificationDefinitionsUrls = LoadApacheHopUrls();

            var results = new List<HealthCheckResult>();

            foreach (var resourceName in notificationDefinitionsUrls)
            {
                var response = CheckEndpointAsync((configManager.Value.ApacheHopBaseUrl + "/hop/webService/?service=" + resourceName + "&checkStatus=true"), string.Empty);
                results.Add(response);
            }

            LogResults(results);

            return Task.CompletedTask;
        }

        private HealthCheckResult CheckEndpointAsync(string url, string payload)
        {
            int attempts = 1;
            if (genericDataTransferStudySettings.MaxNumberOfRetriesForApacheHealthCheck <= 0)
            {
                Logger.Info($"Max number of attemts is lower or equal to 0 (value: {genericDataTransferStudySettings.MaxNumberOfRetriesForApacheHealthCheck}." +
                            $" No attempts will be performed." +
                            $" MaxNumberOfRetriesForApacheHealthCheck setting needs to be higher than 0.");
                return null;
            }

            while (attempts <= genericDataTransferStudySettings.MaxNumberOfRetriesForApacheHealthCheck)
            {
                Logger.Info($"Started attempt {attempts} of sending to {url}.");
                attempts++;
                try
                {
                    using (var httpClient = httpClientFactory.CreateClient())
                    {
                        if (genericDataTransferStudySettings.Timeout > 0)
                        {
                            httpClient.Timeout = new TimeSpan(0, 0, genericDataTransferStudySettings.Timeout.Value);
                        }

                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            scheme: "Basic",
                            parameter: Convert.ToBase64String(
                                inArray: ASCIIEncoding.ASCII.GetBytes(
                                    s: "{0}:{1}"
                                        .F(configManager.Value.ApacheHopUsername, configManager.Value.ApacheHopPassword))));

                        var request = new HttpRequestMessage(HttpMethod.Post, url);
                        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                        var response = httpClient.SendAsync(request).Result;

                        //A response different than 200 means endpoint not accessible
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            return new HealthCheckResult
                            {
                                EndpointUrl = url,
                                IsHealthy = false,
                                Status = "Failed",
                                ErrorDescription = $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}"
                            };
                        }

                        return new HealthCheckResult
                        {
                            EndpointUrl = url,
                            IsHealthy = true,
                            Status = "Success",
                            ErrorDescription = $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}"
                        };
                    }
                }
                catch (TaskCanceledException ex)
                {
                    Logger.Error($"Error during attempt {attempts} while sending to {url}. Exception: \n {ex.Message}");
                    if (attempts > genericDataTransferStudySettings.MaxNumberOfRetriesForApacheHealthCheck)
                    {
                        return new HealthCheckResult
                        {
                            EndpointUrl = url,
                            IsHealthy = false,
                            Status = "Timeout",
                            ErrorDescription = ex.Message
                        };
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error during attempt {attempts} while sending to {url}. Exception: \n {ex.Message}");
                    if (attempts > genericDataTransferStudySettings.MaxNumberOfRetriesForApacheHealthCheck)
                    {
                        return new HealthCheckResult
                        {
                            EndpointUrl = url,
                            IsHealthy = false,
                            Status = "Connection error",
                            ErrorDescription = ex.Message
                        };
                    }
                }
            }

            // Shouldn't reach here
            return new HealthCheckResult
            {
                EndpointUrl = url,
                IsHealthy = false,
                Status = "Unknown error",
                ErrorDescription = "Unknown error"
            };
        }

        private List<string> LoadApacheHopUrls()
        {
            var dynamicFields = dynamicFormFieldRegistry.GetDynamicFieldsForForm("GenericIntegrationFields");

            //TODO: NotificationDefinitions should be gathered from the Registry.
            //There is a missing DynamicDataJson assignation in SyncNotificationDefinitionRegistry IRT.Domain.Notifications.NotificationDefinitionDatabaseSyncService
            var notificationDefinition = dbContext.Set<NotificationDefinitionSqlView>()
                .Where(x => x.DynamicDataJson != null)
                .ToList();

            if (notificationDefinition.Count == 0)
            {
                Logger.Info($"No Notification definitions found with a DynamicDataJson populated.");

                return null;
            }

            var savedDynamicDataValues = notificationDefinition
                    .Select(x => JsonConvert.DeserializeObject<Dictionary<string, string>>(x.DynamicDataJson ?? "{}"));

            var apacheHopUrlDynamicField = dynamicFields
                .SingleOrDefault(x => x.FieldName.Equals(ApacheHopUrlFieldName, StringComparison.OrdinalIgnoreCase));

            var urls = savedDynamicDataValues
                .Select(dynamicValues => dynamicValues.TryGetValue(apacheHopUrlDynamicField.FieldName, out var value)
                    ? value
                    : null)
                .Where(x => !x.IsNullOrEmpty())
                .ToList();

            Logger.Info($"Discovered a total of {urls.Count} Apache Hop URLs.");

            return urls;
        }

        private void LogResults(List<HealthCheckResult> results)
        {
            foreach (var r in results)
            {
                var status = r.IsHealthy ? "HEALTHY" : "UNHEALTHY";
                Logger.Info($"[{status}] {r.EndpointUrl}: " +
                            $"{r.Status} {(string.IsNullOrWhiteSpace(r.ErrorDescription) ? "" : "- " + r.ErrorDescription)}");
            }
        }
    }

    // Result object to capture the outcome
    public class HealthCheckResult
    {
        public string EndpointUrl { get; set; }

        public bool IsHealthy { get; set; }

        public string Status { get; set; }

        public string ErrorDescription { get; set; }
    }
}
