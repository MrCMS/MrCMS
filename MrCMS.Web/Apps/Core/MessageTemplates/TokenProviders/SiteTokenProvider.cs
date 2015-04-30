using System;
using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Core.MessageTemplates.TokenProviders
{
    public class SiteTokenProvider : ITokenProvider
    {
        private readonly Site _site;

        public SiteTokenProvider(Site site)
        {
            _site = site;
        }

        private IDictionary<string, Func<string>> _tokens;

        public IDictionary<string, Func<string>> Tokens
        {
            get { return _tokens = _tokens ?? GetTokens(); }

        }

        private IDictionary<string, Func<string>> GetTokens()
        {
            return new Dictionary<string, Func<string>>
            {
                {"SiteName", () => _site != null ? _site.Name : null},
                {
                    "SiteUrl",
                    () =>
                        _site != null
                            ? string.Format("http://{0}", _site.BaseUrl)
                            : null
                },
            };
        }
    }
}