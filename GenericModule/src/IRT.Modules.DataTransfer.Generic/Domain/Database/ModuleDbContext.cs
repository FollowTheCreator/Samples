using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.DynamicDataTransferSettings;
using IRT.Modules.DataTransfer.Generic.Domain.SqlViews.DynamicSetting;
using Kernel.Audit.Services;
using Kernel.EntityFramework.Extensions;
using Kernel.EntityFramework.Settings;

namespace IRT.Modules.DataTransfer.Generic.Domain.Database
{
    public class ModuleDbContext : IRTDbContext
    {
        public ModuleDbContext(DbContextOptions<IRTDbContext> options, IAuditUserProvider auditUserProvider, string connectionString, IOptions<DatabaseSettings> databaseSettings)
            : base(options, auditUserProvider, connectionString, databaseSettings)
        {
        }

        public DbSet<DynamicDataTransferSettingSqlView> DynamicDataTransferSettings { get; set; }

        public DbSet<DynamicSettingSqlView> DynamicSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddModuleEntities<Module>();

            base.OnModelCreating(modelBuilder);

            //for sqlite in memory in unit tests we still need the entities from IRT Core
            if (databaseSettings.DatabaseType == Kernel.EntityFramework.Settings.DatabaseTypeConfiguration.DatabaseType.SqlServer)
            {
                modelBuilder.ExcludeOtherEntitiesFromMigrations(typeof(Module).Assembly);
            }
        }
    }
}
