using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Frameworks.ExtendedProperties.Attributes;
using Frameworks.ExtendedProperties.Services.Implementations;
using IRT.Domain.Aggregates.SelfSupportWorkflowOrchestrator.ValueObjects;
using IRT.Domain.Services.Impl;
using IRT.Domain.ViewsSql.ExtendedProperty;
using IRT.Modules.DataTransfer.Generic.Domain.Providers.OptionsProviders;
using IRT.Modules.DataTransfer.Generic.Domain.Providers.ShouldGenerateNotificationProviders.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings.OptionProviders;
using ResourceKeys = IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings.Resources.NotificationGenerationSettings.ResourceNames;
using SettingsResources = IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings.Resources.NotificationGenerationSettings;

namespace IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.StudySettings
{
    [ExtendsEntity(typeof(EntitySqlView), typeof(ExtendedPropertiesDataService<ExtendedPropertySqlView>))]
    [DesignOnly(true)]
    public class NotificationGenerationSettings
    {
        [Display(
            Order = 1,
            Name = ResourceKeys.Name_Name,
            ResourceType = typeof(SettingsResources))]
        [Column(nameof(EntitySqlView.Name))]
        [Required]
        [Unique]
        [MaxLength(256)]
        public string Name { get; set; }

        [Display(
            Order = 2,
            Name = ResourceKeys.NotificationDefinition_Name,
            Description = ResourceKeys.NotificationDefinition_Description,
            ResourceType = typeof(SettingsResources))]
        [Column(nameof(EntitySqlView.Name))]
        [Required]
        [EditBehavior(OptionsProviderType = typeof(NotificationDefinitionOptionsProvider))]
        public string NotificationDefinition { get; set; }

        [Display(
            Order = 3,
            Name = ResourceKeys.GenerationOrder_Name,
            Description = ResourceKeys.GenerationOrder_Description,
            ResourceType = typeof(SettingsResources))]
        [Required]
        [Range(1, int.MaxValue)]
        [DisplayBehavior(Width = 125, SortOrder = 1)]
        public int GenerationOrder { get; set; }

        [Display(
            Order = 4,
            Name = ResourceKeys.ShouldGenerateFormProviderId_Name,
            Description = ResourceKeys.ShouldGenerateFormProviderId_Description,
            ResourceType = typeof(SettingsResources))]
        [Required]
        [EditBehavior(OptionsProviderType = typeof(OptionsProvider<IShouldGenerateNotificationPolicyRepository>))]
        public string ShouldGenerateFormProviderId { get; set; }

        [Display(
            Order = 5,
            Name = ResourceKeys.VisitId_Name,
            Description = ResourceKeys.VisitId_Description,
            ResourceType = typeof(SettingsResources))]
        [EditBehavior(OptionsProviderType = typeof(VisitsOptionsProvider))]
        public string[] VisitId { get; set; }

        [Display(
            Order = 6,
            Name = ResourceKeys.SubjectVisitCapturedData_Name,
            Description = ResourceKeys.SubjectVisitCapturedData_Description,
            ResourceType = typeof(SettingsResources))]
        [EditBehavior(OptionsProviderType = typeof(SelfSupportFieldsOptionsProvider))]
        [DisplayBehavior(Width = 325)]
        public IEnumerable<string> SubjectVisitCapturedDataPoints { get; set; }

        [Display(
            Order = 7,
            Name = ResourceKeys.ValueComparison_Name,
            Description = ResourceKeys.ValueComparison_Description,
            ResourceType = typeof(SettingsResources))]
        public string ValueToCompare { get; set; }

        [Display(
            Order = 8,
            Name = ResourceKeys.SelfSupportAction_Name,
            Description = ResourceKeys.SelfSupportAction_Description,
            ResourceType = typeof(SettingsResources))]
        [EditBehavior(OptionsProviderType = typeof(EnumStringValueOptionsProvider<SubjectSelfSupportDataChangeType>))]
        public IEnumerable<string> SelfSupportActions { get; set; }
    }
}