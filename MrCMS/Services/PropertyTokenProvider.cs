using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MrCMS.Entities;

namespace MrCMS.Services
{
    public class PropertyTokenProvider<T> : ITokenProvider<T>
    {
        private IDictionary<string, Func<T, string>> _tokens;

        public IDictionary<string, Func<T, string>> Tokens { get { return _tokens = _tokens ?? GetTokens(); } }

        private IDictionary<string, Func<T, string>> GetTokens()
        {
            var propertyInfos = typeof (T).GetProperties().Where(info =>
                                                                 info.CanRead && !info.GetIndexParameters().Any() && !info.GetGetMethod().IsStatic &&
                                                                 !(info.PropertyType.IsGenericType &&
                                                                   info.PropertyType.GetGenericArguments()
                                                                       .Any( arg => arg.IsSubclassOf(typeof (SystemEntity)))) &&
                                                                 !info.PropertyType.IsSubclassOf(typeof (SiteEntity)))
                                          .ToList();

            return propertyInfos.ToDictionary<PropertyInfo, string, Func<T, string>>(propertyInfo => propertyInfo.Name, propertyInfo => (arg => Convert.ToString(propertyInfo.GetValue(arg, null))));
        }
    }
}