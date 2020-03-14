using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Services;

namespace MrCMS.Messages
{
    public class PropertyTokenProvider<T> : ITokenProvider<T>
    {
        private IDictionary<string, Func<T, Task<string>>> _tokens;

        public IDictionary<string, Func<T, Task<string>>> Tokens { get { return _tokens ??= GetTokens(); } }

        private IDictionary<string, Func<T, Task<string>>> GetTokens()
        {
            var propertyInfos = typeof(T).GetProperties().Where(info =>
                    info.CanRead && !info.GetIndexParameters().Any() && !info.GetGetMethod().IsStatic &&
                    !(info.PropertyType.IsGenericType &&
                      info.PropertyType.GetGenericArguments()
                          .Any(arg => arg.IsSubclassOf(typeof(SystemEntity)))) &&
                    !info.PropertyType.IsSubclassOf(typeof(SiteEntity)))
                .ToList();

            return propertyInfos.ToDictionary<PropertyInfo, string, Func<T, Task<string>>>(
                propertyInfo => propertyInfo.Name,
                propertyInfo => arg => Task.FromResult(Convert.ToString(propertyInfo.GetValue(arg, null))));
        }
    }
}