using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;

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
    }
}