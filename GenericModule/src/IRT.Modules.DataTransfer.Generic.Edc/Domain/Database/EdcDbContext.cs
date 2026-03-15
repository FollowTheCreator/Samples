using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using IRT.Domain;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.ItemGroup;
using IRT.Modules.DataTransfer.Generic.Edc.Domain.SqlViews.RepeatKeys.StudyEvent;
using Kernel.Audit.Services;
using Kernel.EntityFramework.Extensions;
using Kernel.EntityFramework.Settings;

namespace IRT.Modules.DataTransfer.Generic.Edc.Domain.Database
{
    public class EdcDbContext : IRTDbContext
    {
        public EdcDbContext(DbContextOptions<IRTDbContext> options, IAuditUserProvider auditUserProvider, string connectionString, IOptions<DatabaseSettings> databaseSettings)
            : base(options, auditUserProvider, connectionString, databaseSettings)
        {
        }

        public DbSet<GenericStudyEventRepeatKeySqlView> GenericStudyEventRepeatKeys { get; set; }

        public DbSet<GenericStudyEventRepeatKeyLastUsedSqlView> GenericStudyEventRepeatKeysLastUsed { get; set; }

        public DbSet<GenericItemGroupRepeatKeySqlView> GenericItemGroupRepeatKeys { get; set; }

        public DbSet<GenericItemGroupRepeatKeyLastUsedSqlView> GenericItemGroupRepeatKeysLastUsed { get; set; }

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
