using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MrCMS.Helpers;
using MrCMS.Messages;
using Ninject;

namespace MrCMS.Services
{
    public class MessageTemplateParser : IMessageTemplateParser
    {
        private static readonly MethodInfo GetMessageTemplateMethod = typeof (MessageTemplateParser)
            .GetMethodExt("GetAllTokens");

        private readonly IKernel _kernel;

        public MessageTemplateParser(IKernel kernel)
        {
            _kernel = kernel;
        }

        public string Parse<T>(string template, T instance)
        {
            var stringBuilder = new StringBuilder(template);
            IEnumerable<ITokenProvider<T>> tokenProviders = _kernel.GetAll<ITokenProvider<T>>();

            foreach (var token in tokenProviders.SelectMany(tokenProvider => tokenProvider.Tokens))
            {
                stringBuilder.Replace("{" + token.Key + "}", token.Value(instance));
            }

            return stringBuilder.ToString();
        }

        public List<string> GetAllTokens<T>()
        {
            IEnumerable<ITokenProvider<T>> tokenProviders = _kernel.GetAll<ITokenProvider<T>>();
            return tokenProviders.SelectMany(provider => provider.Tokens.Select(pair => pair.Key)).ToList();
        }

        public List<string> GetAllTokens(MessageTemplateBase template)
        {
            var tokens = new List<string>();
            tokens.AddRange(GetAllTokens(template.ModelType));
            tokens.AddRange(GetAllStandardTokens());
            return tokens;
        }

        private List<string> GetAllTokens(Type type)
        {
            if (type == null)
                return new List<string>();
            return GetMessageTemplateMethod.MakeGenericMethod(type).Invoke(this, new object[] {}) as List<string>;
        }

        public List<string> GetAllStandardTokens()
        {
            IEnumerable<ITokenProvider> tokenProviders = _kernel.GetAll<ITokenProvider>();
            return tokenProviders.SelectMany(provider => provider.Tokens.Select(pair => pair.Key)).ToList();
        }
    }
}