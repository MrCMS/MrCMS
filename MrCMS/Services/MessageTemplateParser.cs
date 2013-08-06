using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MrCMS.Entities;
using Ninject;
using Ninject.Parameters;

namespace MrCMS.Services
{
    public class MessageTemplateParser : IMessageTemplateParser
    {
        private readonly IKernel _kernel;

        public MessageTemplateParser(IKernel kernel)
        {
            _kernel = kernel;
        }

        public string Parse<T>(string template, T instance)
        {
            var stringBuilder = new StringBuilder(template);
            var tokenProviders = _kernel.GetAll<ITokenProvider<T>>();

            foreach (var token in tokenProviders.SelectMany(tokenProvider => tokenProvider.Tokens))
            {
                stringBuilder.Replace("{" + token.Key + "}", token.Value(instance));
            }

            return stringBuilder.ToString();
        }

        public List<string> GetAllTokens<T>()
        {
            var tokenProviders = _kernel.GetAll<ITokenProvider<T>>();
            return tokenProviders.SelectMany(provider => provider.Tokens.Select(pair => pair.Key)).ToList();
        }
    }

    public interface ITokenProvider<T>
    {
        IDictionary<string, Func<T, string>> Tokens { get; }
    }

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