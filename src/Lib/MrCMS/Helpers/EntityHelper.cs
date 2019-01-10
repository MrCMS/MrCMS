using System;
using System.Linq;
using MrCMS.Entities;

namespace MrCMS.Helpers
{
    public static class EntityHelper
    {
        public static T ShallowCopy<T>(this T entity) where T : SystemEntity
        {
            var type = entity.GetType();
            var propertyInfos =
                type.GetProperties()
                    .Where(
                        info =>
                        info.CanWrite &&
                        !(info.PropertyType.IsGenericType &&
                          info.PropertyType.GetGenericArguments().Any(arg => arg.IsSubclassOf(typeof (SystemEntity)))) &&
                        !info.PropertyType.IsSubclassOf(typeof (SystemEntity)));

            var shallowCopy = Activator.CreateInstance(type) as T;
            foreach (var propertyInfo in propertyInfos)
            {
                propertyInfo.SetValue(shallowCopy, propertyInfo.GetValue(entity, null), null);
            }
            shallowCopy.Id = 0;
            return shallowCopy;
        }
    }
}