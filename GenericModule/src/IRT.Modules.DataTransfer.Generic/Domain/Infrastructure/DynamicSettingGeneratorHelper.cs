using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Kernel.Utilities.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Domain.Infrastructure
{
    public static class DynamicSettingGeneratorHelper
    {
        private static string extendedPropertiesTypeName = "DynamicGenericDataTransferStudySettings";
        public static string AssemblyName = "IRT.Modules.DataTransfer.Generic.DynamicSettings";

        private static Lazy<ModuleBuilder> moduleBuilder = new Lazy<ModuleBuilder>(GetModuleBuilder());
        private static ConcurrentDictionary<string, TypeBuilder> typeBuilderLookup = new ConcurrentDictionary<string, TypeBuilder>();

        private static ModuleBuilder GetModuleBuilder()
        {
            var assemblyName = new AssemblyName(AssemblyName);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            return moduleBuilder;
        }

        public static CustomAttributeBuilder EmitAttribute(Type attributeType, Type[] constructorTypes, object[] constructorValues)
        {
            ConstructorInfo classCtorInfo = attributeType.GetConstructor(constructorTypes);
            var caBuilder = new CustomAttributeBuilder(
                                classCtorInfo,
                                constructorValues);
            return caBuilder;
        }

        public static CustomAttributeBuilder EmitAttribute(Type attributeType, PropertyInfo[] propertyTypes, object[] propertyValues)
        {
            var customAttribute = new CustomAttributeBuilder(
                attributeType.GetConstructor(Type.EmptyTypes), // constructor selection
                new object[0], // constructor arguments - none
                propertyTypes,
                propertyValues);

            return customAttribute;
        }

        public static TypeBuilder EmitType(string typeName, IEnumerable<CustomAttributeBuilder> customAttributeBuilders)
        {
            if (typeBuilderLookup.TryGetValue(typeName, out var type))
            {
                return type;
            }

            TypeBuilder typeBuilder = moduleBuilder.Value.DefineType(typeName, TypeAttributes.Public);
            foreach (var attributeBuilder in customAttributeBuilders)
            {
                typeBuilder.SetCustomAttribute(attributeBuilder);
            }

            typeBuilderLookup.TryAdd(typeName, typeBuilder);

            return typeBuilder;
        }

        public static PropertyBuilder EmitProperty(
            TypeBuilder typeBuilder,
            Type propertyType,
            string propertyName,
            IEnumerable<CustomAttributeBuilder> customAttributeBuilders)
        {
            var fieldBuilder = typeBuilder.DefineField(char.ToLower(propertyName[0]) + propertyName.Substring(1), propertyType, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            foreach (var attributeBuilder in customAttributeBuilders)
            {
                propertyBuilder.SetCustomAttribute(attributeBuilder);
            }

            var getMethodBuilder = typeBuilder.DefineMethod(
                "get_{0}".F(propertyName),
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                propertyType,
                Type.EmptyTypes);

            var iLGenerator = getMethodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            iLGenerator.Emit(OpCodes.Ret);

            var setMethodBuilder = typeBuilder.DefineMethod(
                "set_{0}".F(propertyName),
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null,
                new[] { propertyType });

            iLGenerator = setMethodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            iLGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);

            return propertyBuilder;
        }

        public static string GetUniqueTypeName(Guid dynamicSettingId)
        {
            return AssemblyName + "." + extendedPropertiesTypeName + "_" + dynamicSettingId.ToString("N").Substring(0, 8);
        }
    }
}
