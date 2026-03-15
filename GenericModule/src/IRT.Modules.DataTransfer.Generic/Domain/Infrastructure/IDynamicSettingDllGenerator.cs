using System;
using System.Reflection.Emit;

namespace IRT.Modules.DataTransfer.Generic.Domain.Infrastructure
{
    public interface IDynamicSettingDllGenerator
    {
        public void RegisterType(TypeBuilder typeBuilder, string entityTypeName);

        public TypeBuilder AddType(string entityTypeName, Guid dynamicSettingId);

        public void AddProperty(TypeBuilder typeBuilder,
            string extendedPropertyName,
            string defaultValue,
            string name,
            string description);
    }
}
