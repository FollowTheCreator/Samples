using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting.Events;
using IRT.Modules.DataTransfer.Generic.Domain.Database;
using Kernel.DDD.Dispatching;
using Unity;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews.DynamicSetting
{
    public class DynamicSettingSqlViewHandler : IEventHandler
    {
        [Dependency]
        public ModuleDbContext Db { get; set; }

        public void Handle(DynamicSettingCreated e)
        {
            var dynamicSetting = new DynamicSettingSqlView
            {
                DynamicSettingId = e.DynamicSettingId,
                Name = e.Name,
                Description = e.Description,
                DefaultValue = e.DefaultValue,
                EntityTypeName = e.EntityTypeName,
                ExtendedPropertyName = e.ExtendedPropertyName
            };

            Db.DynamicSettings.Add(dynamicSetting);
            Db.SaveChanges();
        }
    }
}
