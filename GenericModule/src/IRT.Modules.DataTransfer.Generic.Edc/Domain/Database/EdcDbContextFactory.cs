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

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Database
{
    public class EdcDbContextFactory : ModuleDesignTimeDbContextFactory<EdcDbContext, IRTDbContext>
    {
        protected override EdcDbContext CreateDbContext(IConfiguration configuration, DbContextOptions<IRTDbContext> dbContextOptions, IOptions<DatabaseSettings> databaseSettingsOptions)
        {
            return new EdcDbContext(
                dbContextOptions,
                new SystemAuditUserProvider(),
                configuration.GetConnectionString(nameof(EdcDbContext)),
                databaseSettingsOptions);
        }

        protected override void RegisterServices(IUnityContainer unityContainer)
        {
            unityContainer.RegisterInstance<IHostEnvironment>(new DesignTimeHostEnvironment());
            unityContainer.RegisterInstance<IHttpContextAccessor>(new DesignTimeHttpContextAccessor());
        }
    }
}