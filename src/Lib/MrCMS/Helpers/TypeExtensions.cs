using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

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

        public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            dynamic awaitable = @this.Invoke(obj, parameters);
            await awaitable;
            return awaitable.GetAwaiter().GetResult();
        }
        public static async Task InvokeVoidAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            dynamic awaitable = @this.Invoke(obj, parameters);
            await awaitable;
        }
    }
}