using System;
using System.ComponentModel.Composition;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Frameworks.Notifications;
using IRT.Domain;
using IRT.Domain.Aggregates.Subject.Events;
using IRT.Domain.Aggregates.Subject.Events.SelfSupport;
using IRT.Modules.DataTransfer.Generic.Domain.Configuration;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Implementations;
using IRT.Modules.DataTransfer.Generic.Domain.Services.Interfaces;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Database;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Resources;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.Services.Implementations;
using Kernel.Audit.Services;
using Kernel.EntityFramework.Interfaces;
using Kernel.EntityFramework.Settings;
using Kernel.Globalization.Providers;
using Kernel.Globalization.Registries;
using Kernel.Modularity;
using Kernel.Modularity.Interfaces;
using Unity;
using Unity.Lifetime;

namespace IRT.Modules.DataTransfer.Generic.Edc
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
        }

        private void ConfigureServices(IUnityContainer container)
        {
            container.RegisterType<INotificationSenderService, RaveWebApiNotificationSenderService>(typeof(RaveWebApiNotificationSenderService).FullName);

            container.RegisterType<INotificationSenderService, VeevaWebApiNotificationSenderService>(typeof(VeevaWebApiNotificationSenderService).FullName);
        }

        private void ConfigureDatabase(IUnityContainer container)
        {
            container.RegisterType<IDbSeedingService<EdcDbContext>, EdcDbSeedingService>();

            container.RegisterFactory<EdcDbContext>(cont =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<IRTDbContext>();
                optionsBuilder.UseApplicationServiceProvider(cont.Resolve<IServiceProvider>());
                optionsBuilder.UseInternalServiceProvider(cont.Resolve<IServiceProvider>());

                var databaseSettings = cont.Resolve<IOptions<DatabaseSettings>>();
                var irtAppConfigMgr = cont.Resolve<IOptions<IrtAppConfigManager>>().Value;
                var auditUserProvider = cont.Resolve<IAuditUserProvider>();

                var context = new EdcDbContext(optionsBuilder.Options, auditUserProvider, irtAppConfigMgr.ConnectionStringDbIrt, databaseSettings);
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                return context;
            }, new HierarchicalLifetimeManager());
        }

        public void Initialize(IUnityContainer container, DbContext dbContext)
        {
            var ctx = container.Resolve<EdcDbContext>();
            ctx.Database.Migrate();
        }

        private static void RegisterContextsGenerators(IUnityContainer container)
        {
            var contextsGeneratorFactoryRegistry = container.Resolve<IContextsGeneratorRegistrationService>();

            contextsGeneratorFactoryRegistry.Register<
                SubjectVisitPerformed,
                SubjectVisitPerformedContextsGenerator>();

            contextsGeneratorFactoryRegistry.Register<
                SelfSupportChangeRequestProcessed,
                SelfSupportChangeRequestProcessedContextsGenerator>();
        }

        private void ConfigureResources(IUnityContainer container)
        {
            var resourceManager = container.Resolve<IResourceManagerRegistry>();
            resourceManager.RegisterResourceManagers(new AssemblyResourceProvider(typeof(Module).Assembly));
            resourceManager.RegisterFallbackResource<IRT.Domain.Resources.FunctionOperationsResources, FunctionOperationResources>();
        }
    }
}
