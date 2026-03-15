using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using IRT.Domain;
using Kernel.AspNetMvc.DesignTime;
using Kernel.Audit.Services;
using Kernel.EntityFramework.DesignTime;
using Kernel.EntityFramework.Settings;
using Unity;

namespace IRT.Modules.DataTransfer.Generic.Domain.Database
{
    public class ModuleDbContextFactory : ModuleDesignTimeDbContextFactory<ModuleDbContext, IRTDbContext>
    {
        protected override ModuleDbContext CreateDbContext(IConfiguration configuration, DbContextOptions<IRTDbContext> dbContextOptions, IOptions<DatabaseSettings> databaseSettingsOptions)
        {
            return new ModuleDbContext(
                dbContextOptions,
                new SystemAuditUserProvider(),
                configuration.GetConnectionString(nameof(ModuleDbContext)),
                databaseSettingsOptions);
        }

        protected override void RegisterServices(IUnityContainer unityContainer)
        {
            unityContainer.RegisterInstance<IHostEnvironment>(new DesignTimeHostEnvironment());
            unityContainer.RegisterInstance<IHttpContextAccessor>(new DesignTimeHttpContextAccessor());
        }
    }
}