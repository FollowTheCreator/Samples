using System.Reflection.Emit;
using IRT.Domain.ViewsSql;
using IRT.Modules.DataTransfer.Generic.Domain.Aggregates.DynamicSetting.Events;
using IRT.Modules.DataTransfer.Generic.Domain.Infrastructure;

namespace IRT.Modules.DataTransfer.Generic.Domain.SqlViews.DynamicSetting
{
    public class DynamicSettingDllHandler : SqlViewHandlerBase
    {
        private readonly IDynamicSettingDllGenerator dynamicSettingDllGenerator;

        public DynamicSettingDllHandler(
            IDynamicSettingDllGenerator dynamicSettingDllGenerator)
        {
            this.dynamicSettingDllGenerator = dynamicSettingDllGenerator;
        }

        public void Handle(DynamicSettingDllCreated e)
        {
            TypeBuilder typeBuilder = dynamicSettingDllGenerator.AddType(e.EntityTypeName, e.DynamicSettingId);

            dynamicSettingDllGenerator.AddProperty(typeBuilder,
                e.ExtendedPropertyName,
                e.DefaultValue,
                e.Name,
                e.Description);

            dynamicSettingDllGenerator.RegisterType(typeBuilder, e.EntityTypeName);
        }
    }
}
