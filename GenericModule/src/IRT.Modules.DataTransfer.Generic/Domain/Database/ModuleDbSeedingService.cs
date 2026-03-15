using Kernel.EntityFramework.Interfaces;

namespace IRT.Modules.DataTransfer.Generic.Domain.Database
{
    public class ModuleDbSeedingService : IDbSeedingService<ModuleDbContext>
    {
        public ModuleDbSeedingService()
        {
        }

        public void Seed(ModuleDbContext context)
        {
        }
    }
}