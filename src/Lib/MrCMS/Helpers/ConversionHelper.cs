using System;

namespace MrCMS.Helpers
{
    public static class ConversionHelper
    {
        public static T ConvertTo<T>(this string value)
        {
            if (!typeof(T).IsGenericType)
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            Type genericTypeDefinition = typeof(T).GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(Nullable<>))
            {
                return (T)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)));
            }
            return default(T);
        }
    }
}