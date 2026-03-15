using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection.Emit;
using System.Resources;
using CommonServiceLocator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Frameworks.ExtendedProperties.Configuration;
using Frameworks.ExtendedProperties.Services.Implementations;
using Frameworks.ExtendedProperties.ValueObjects;
using Frameworks.Notifications;
using IRT.Domain;
using IRT.Domain.Aggregates.Study;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Domain.Infrastructure.Modularity;
using IRT.Domain.Services.Interfaces;
using IRT.Domain.ValueObjects.Resources;
using IRT.Domain.ViewsSql.ExtendedProperty;
using IRT.Domain.ViewsSql.Visit;
using IRT.Modules.DataTransfer.Generic.Areas.StudyAdministration.Controllers;
using IRT.Modules.DataTransfer.Generic.Domain.Configuration;
using IRT.Modules.DataTransfer.Generic.Domain.Database;
using IRT.Modules.DataTransfer.Generic.Domain.DynamicFields;
using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;
using IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices;
using IRT.Modules.DataTransfer.Generic.Domain.Notifications.DataServices.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.Notifications.Templating;
using IRT.Modules.DataTransfer.Generic.Domain.Providers.ShouldGenerateNotificationProviders;
using IRT.Modules.DataTransfer.Generic.Domain.Providers.ShouldGenerateNotificationProviders.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.Resources;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.DynamicDataTransferSettings;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDefinition;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.GenericNotificationDependency;
using IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.Resource;
using IRT.Modules.DataTransfer.Generic.Properties;
using Kernel.AspNetMvc.DynamicData.Services.Interfaces;
using Kernel.AspNetMvc.Menu;
using Kernel.AspNetMvc.Security;
using Kernel.AspNetMvc.ValueObjects;
using Kernel.Audit.Services;
using Kernel.EntityFramework.Interfaces;
using Kernel.EntityFramework.Settings;
using Kernel.Globalization.Entities;
using Kernel.Globalization.Providers;
using Kernel.Globalization.Registries;
using Kernel.Modularity;
using Kernel.Modularity.Interfaces;
using Kernel.Utilities;
using Kernel.Utilities.Constants;
using Kernel.Utilities.ValueObjects;
using Unity;
using Unity.Lifetime;
using Unity.RegistrationByConvention;
using Operations = IRT.Modules.DataTransfer.Generic.Domain.Operations;

namespace IRT.Modules.DataTransfer.Generic
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Module : IModule, IIntegrationModule
    {
        public static readonly string ModuleId = typeof(Module).Assembly.GetName().Name;

        public void Configure(IServiceCollection services)
        {
            services.Configure<DTGenericConfigManager>(options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var configuration = serviceProvider.GetService<IConfiguration>();

                options.ApacheHopUsername = configuration.GetSection("DTGenericConfigManager:ApacheHopUsername").Value;
                options.ApacheHopPassword = configuration.GetSection("DTGenericConfigManager:ApacheHopPassword").Value;
                options.ApacheHopBaseUrl = configuration.GetSection("DTGenericConfigManager:ApacheHopBaseUrl").Value;
            });
        }

        public void Configure(IUnityContainer container)
        {
            ConfigureResources(container);

            ConfigureServices(container);

            RegisterContextsGenerators(container);

            ConfigureDatabase(container);

            RegisterDynamicDataTransferSettingQueriesAndDbSets(container);

            ConfigureMenuItems(container);

            ConfigureExtendedProperties(container);
        }

        private void ConfigureServices(IUnityContainer container)
        {
            container.RegisterType<IShouldGenerateNotificationPolicyRepository, ShouldGenerateNotificationPolicyRepository>(new ContainerControlledLifetimeManager());

            container.RegisterType<IGenericNotificationDataServiceRepository, GenericNotificationDataServiceRepository>(new ContainerControlledLifetimeManager());

            container.RegisterType<IGenericNotificationDefinitionService, GenericNotificationDefinitionService>();

            container.RegisterType<INotificationGenerationService, NotificationGenerationService>();

            container.RegisterType<IContextsGeneratorRegistrationService, ContextsGeneratorRegistrationService>();

            container.RegisterType<IContextsGeneratorFactory, ContextsGeneratorFactory>();

            container.RegisterType<IEventGenerationService, EventGenerationService>();

            container.RegisterType<IGenericNotificationDependencyService, GenericNotificationDependencyService>();

            container.RegisterType<INotificationDefinitionRenderer, GenericNotificationDefinitionRenderer>(typeof(GenericNotificationDefinitionRenderer).FullName);

            container.RegisterType<INotificationDefinitionRenderer, GenericNotificationDefinitionRenderer>(typeof(GenericNotificationDefinitionRenderer).FullName);

            container.RegisterType<Domain.Services.Interfaces.IFailReasonsService, FailReasonsService>();

            container.RegisterType<IDynamicSettingDllGenerator, DynamicSettingDllGenerator>();

            container.RegisterType<IFormFieldContributor, GenericIntegrationFields>(typeof(GenericIntegrationFields).FullName);
        }

        private void ConfigureExtendedProperties(IUnityContainer container)
        {
            var functionConfigurator = container.Resolve<IExtendedPropertiesFunctionConfigurator>();

            functionConfigurator
                .ConfigureCrudFunction<EntitySqlView, Domain.ValueObjects.StudySettings.NotificationGenerationSettings>(Operations.GenericDataTransfer.ViewManageNotificationsGenerationOperation.Id)
                .WithEntityIdentifier((valueProvider, entityDto) =>
                    valueProvider.GetValue<EntitySqlView, Domain.ValueObjects.StudySettings.NotificationGenerationSettings, string>(x => x.Name, entityDto.EntityId))
                 .WithCreateBehavior()
                .WithUpdateBehavior()
                .WithDeleteBehavior()
                .WithImportBehavior()
                .WithExportBehavior(
                    new StringResourceDescriptor(typeof(Properties.Resources), Resources.ResourceNames.SettingsExportFileName),
                    IrtConstants.CsvFileExtension)
                .WithSubMenu(Operations.GenericDataTransfer.NotificationGenerationManagementDefaultOperationIds);

            functionConfigurator.ConfigureEntityBehavior<VisitSqlView>(
                entityTypeDisplayName: new StringResourceDescriptor(
                    typeof(IRT.Domain.Resources.ExtendedPropertiesResources),
                    IRT.Domain.Resources.ExtendedPropertiesResources.ResourceNames.VisitEntityTypeDisplayName),
                entityDisplayNameResourceType: null,
                entityQueryBuilder: (db, name) => ServiceLocator.Current.GetInstance<IVisitNameLocalizationHelper>()
                    .GetVisitsWithLocalizedNames()
                    .Select(x => new EntityDisplayInfo
                    {
                        EntityId = x.VisitId,
                        DefaultEntityDisplayName = string.Concat(x.VisitId, StringConstants.Space, "(", x.VisitName, ")"),
                        EntityTypeName = typeof(VisitSqlView).FullName
                    }))
                .ForAggregate<StudyAggregate>((entityTypeName, extendedPropertiesTypeName, entityId) =>
                    ServiceLocator.Current.GetInstance<NewStudyDataService>()
                        .GetStudyId()
                        .ToString());

            functionConfigurator.ConfigureCrudFunction<ResourceSqlView, DynamicResource>(Operations.GenericDataTransfer.ManageDynamicResourcesOperation.Id)
                .WithEntityIdentifier(
                    entityIdentifier: (valueProvider, entityDto) =>
                    {
                        var key = ReflectionUtility.GetPropertyName<IRT.Domain.ValueObjects.Resources.ResourceExtendedProperties<DynamicResourceIWR>>(x => x.Name);
                        return entityDto.Values.ContainsKey(key) ? (string)entityDto.Values[key] : entityDto.EntityId;
                    },
                    entityTypeDisplayName: new StringResourceDescriptor(typeof(DynamicExtendedPropertiesResources), DynamicExtendedPropertiesResources.ResourceNames.DynamicResourceEntityTypeDisplayName))
                .WithCommandColumn(columnWidth: 100)
                .WithCreateBehavior<DynamicResource.AdditionalLogicExecutor>()
                .WithUpdateBehavior<DynamicResource.AdditionalLogicExecutor>()
                .WithDeleteBehavior<DynamicResource.AdditionalLogicExecutor>()
                .WithImportBehavior<DynamicResource.AdditionalLogicExecutor>()
                .WithExportBehavior(new StringResourceDescriptor(typeof(DynamicExtendedPropertiesResources), DynamicExtendedPropertiesResources.ResourceNames.DynamicResourceExportFileName), IrtConstants.CsvFileExtension);

            functionConfigurator
                .ConfigureCrudFunction<ExtendedPropertiesDataService<DynamicDataTransferSettingSqlView>>(Operations.GenericDataTransfer.ManageDynamicSettingsOperation.Id);
        }

        private void ConfigureDatabase(IUnityContainer container)
        {
            container.RegisterType<IDbSeedingService<ModuleDbContext>, ModuleDbSeedingService>();

            container.RegisterFactory<ModuleDbContext>(cont =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<IRTDbContext>();
                optionsBuilder.UseApplicationServiceProvider(cont.Resolve<IServiceProvider>());
                optionsBuilder.UseInternalServiceProvider(cont.Resolve<IServiceProvider>());

                var databaseSettings = cont.Resolve<IOptions<DatabaseSettings>>();
                var irtAppConfigMgr = cont.Resolve<IOptions<IrtAppConfigManager>>().Value;
                var auditUserProvider = cont.Resolve<IAuditUserProvider>();

                var context = new ModuleDbContext(optionsBuilder.Options, auditUserProvider, irtAppConfigMgr.ConnectionStringDbIrt, databaseSettings);
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                return context;
            }, new HierarchicalLifetimeManager());
        }

        private void RegisterContextsGenerators(IUnityContainer container)
        {
            var contextsGeneratorFactoryRegistry = container.Resolve<IContextsGeneratorRegistrationService>();

            contextsGeneratorFactoryRegistry.Register<
                SubjectVisitPerformed,
                SubjectVisitPerformedContextsGenerator>();

            contextsGeneratorFactoryRegistry.Register<
                SelfSupportChangeRequestProcessed,
                SelfSupportChangeRequestProcessedContextsGenerator>();
        }

        private void RegisterDynamicDataTransferSettingQueriesAndDbSets(IUnityContainer container)
        {
            container.RegisterFactory(
                typeof(IQueryable<DynamicDataTransferSettingSqlView>),
                (cont, type, name) =>
                {
                    var closedType = type.GetGenericArguments()[0];
                    var dbContext = cont.Resolve<ModuleDbContext>();
                    var setMethodInfo = dbContext.GetType().GetMethod("Set", new Type[0]).MakeGenericMethod(closedType);
                    var asNoTrackingMethodInfo = typeof(EntityFrameworkQueryableExtensions).GetMethod("AsNoTracking").MakeGenericMethod(closedType);

                    var dbSet = setMethodInfo.Invoke(dbContext, new object[0]);
                    var queryableEntities = asNoTrackingMethodInfo.Invoke(null, new object[] { dbSet });

                    return queryableEntities;
                },
                new HierarchicalLifetimeManager());

            container.RegisterFactory(
                typeof(DbSet<DynamicDataTransferSettingSqlView>),
                (cont, type, name) =>
                {
                    var closedType = type.GetGenericArguments()[0];
                    var dbContext = cont.Resolve<ModuleDbContext>();
                    var setMethodInfo = dbContext.GetType().GetMethod("Set").MakeGenericMethod(closedType);

                    var dbSet = setMethodInfo.Invoke(dbContext, new object[0]);

                    return dbSet;
                },
                new HierarchicalLifetimeManager());
        }

        private void ConfigureResources(IUnityContainer container)
        {
            var resourceManager = container.Resolve<IResourceManagerRegistry>();
            resourceManager.RegisterResourceManagers(new AssemblyResourceProvider(typeof(Module).Assembly));
            resourceManager.RegisterFallbackResource<IRT.Domain.Resources.FunctionOperationsResources, FunctionOperationResources>();

            AddResourcesScope(container);
        }

        private void ConfigureMenuItems(IUnityContainer container)
        {
            container.Resolve<IResourceManagerRegistry>().RegisterResourceManager(typeof(Resources));

            if (container.IsRegistered<IAdminMenuManager>())
            {
                container.Resolve<IAdminMenuManager>().GetRootMenuItems()
                    .Single(x => x.Id == "Notifications")
                    .AddChildItems(new MenuItem[]
                    {
                        new ActionLinkMenuItem(
                                menuItemId: "ClientNotifications",
                                actionQualifier: ActionQualifier.Create<ClientNotificationsController>(x => x.Index()),
                                textResourceInfo: new StringResourceDescriptor(
                                    resourceType: typeof(Resources),
                                    resourceKey: Resources.ResourceNames.ViewClientNotifications_MenuItemText))
                            .ShouldRenderIf(() => AuthorizationServiceProvider.AuthorizationService.IsAuthorizedFor(Operations.GenericDataTransfer.ViewClientNotifications)),

                        new ActionLinkMenuItem(
                            menuItemId: "ManageNotificationsGeneration",
                            actionQualifier: ActionQualifier.Create<ManageNotificationsGenerationController>(x => x.Index(Operations.GenericDataTransfer.NotificationGenerationManagementDefaultOperationId)),
                            textResourceInfo: new StringResourceDescriptor(
                                resourceType: typeof(Resources),
                                resourceKey: Resources.ResourceNames.ViewManageNotificationsGeneration_MenuItemText))
                            .ShouldRenderIf(() => AuthorizationServiceProvider.AuthorizationService.IsAuthorizedFor(Operations.GenericDataTransfer.ViewManageNotificationsGeneration)),

                        new ActionLinkMenuItem(
                            menuItemId: "ManageDynamicSettings",
                            actionQualifier: ActionQualifier.Create<ManageDynamicSettingsController>(x => x.Index(Operations.GenericDataTransfer.ManageDynamicSettingsOperation.Id)),
                            textResourceInfo: new StringResourceDescriptor(
                                resourceType: typeof(Resources),
                                resourceKey: Resources.ResourceNames.ManageDynamicSettings_MenuItemText))
                            .ShouldRenderIf(() => AuthorizationServiceProvider.AuthorizationService.IsAuthorizedFor(Operations.GenericDataTransfer.ManageDynamicSettings)),

                        new ActionLinkMenuItem(
                            menuItemId: "ManageDynamicResources",
                            actionQualifier: ActionQualifier.Create<ManageDynamicResourcesController>(x => x.Index(Operations.GenericDataTransfer.ManageDynamicResourcesOperation.Id)),
                            textResourceInfo: new StringResourceDescriptor(
                                resourceType: typeof(Resources),
                                resourceKey: Resources.ResourceNames.ManageDynamicResources_MenuItemText))
                            .ShouldRenderIf(() => AuthorizationServiceProvider.AuthorizationService.IsAuthorizedFor(Operations.GenericDataTransfer.ManageDynamicResources))
                    });
            }
        }

        private void AddResourcesScope(IUnityContainer container)
        {
            var resourcesScopeMapper = container.Resolve<IResourcesScopeMapper>();
            resourcesScopeMapper.AddScope(
                ResourcesScope.IWR,
                new List<ResourceManager>
                {
                    DynamicResourceIWR.ResourceManager
                });
        }

        public void Initialize(IUnityContainer container, DbContext dbContext)
        {
            var ctx = container.Resolve<ModuleDbContext>();
            ctx.Database.Migrate();

            SeedDynamicSettings(container, ctx);
        }

        private void SeedDynamicSettings(IUnityContainer container, ModuleDbContext ctx)
        {
            var dynamicSettingDllGenerator = container.Resolve<IDynamicSettingDllGenerator>();

            foreach (var dynamicSetting in ctx.DynamicSettings)
            {
                TypeBuilder typeBuilder = dynamicSettingDllGenerator.AddType(dynamicSetting.EntityTypeName, dynamicSetting.DynamicSettingId);

                dynamicSettingDllGenerator.AddProperty(typeBuilder,
                    dynamicSetting.ExtendedPropertyName,
                    dynamicSetting.DefaultValue,
                    dynamicSetting.Name,
                    dynamicSetting.Description);

                dynamicSettingDllGenerator.RegisterType(typeBuilder, dynamicSetting.EntityTypeName);
            }
        }
    }
}
