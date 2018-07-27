using System;
using System.ComponentModel;
using System.Reflection;

namespace MrCMS.Helpers
{
    public static class TypeExtensions
    {
        public static bool HasDefaultConstructor(this Type type) => type.GetConstructor(Type.EmptyTypes) != null;

        public static string GetDescription(this Type type)
        {
            var descriptionAttribute = type.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttribute == null ? type.Name.BreakUpString() : descriptionAttribute.Description;
        }

        public static object GetDefaultValue(this Type type) =>
            type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
    }
}