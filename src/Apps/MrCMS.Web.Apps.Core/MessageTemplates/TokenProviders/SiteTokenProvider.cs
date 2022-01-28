using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Core.MessageTemplates.TokenProviders
{
    public class SiteTokenProvider : ITokenProvider
    {
        private readonly ICurrentSiteLocator _locator;

        public SiteTokenProvider(ICurrentSiteLocator locator)
        {
            _locator = locator;
        }

        private IDictionary<string, Func<Task<string>>> _tokens;

        public IDictionary<string, Func<Task<string>>> Tokens
        {
            get { return _tokens ??= GetTokens(); }
        }

        private IDictionary<string, Func<Task<string>>> GetTokens()
        {
            return new Dictionary<string, Func<Task<string>>>
            {
                {
                    "SiteName", () =>
                    {
                        var site = _locator.GetCurrentSite();
                        return Task.FromResult(site?.Name);
                    }
                },
                {
                    "SiteUrl",
                    () =>
                    {
                        var site = _locator.GetCurrentSite();
                        return Task.FromResult(site != null
                            ? $"https://{site.BaseUrl}"
                            : null);
                    }
                },
            };
        }
    }
}