using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Core.MessageTemplates.TokenProviders
{
    public class SiteTokenProvider : ITokenProvider
    {
        private readonly IGetCurrentSite _getCurrentSite;

        public SiteTokenProvider(IGetCurrentSite getCurrentSite)
        {
            _getCurrentSite = getCurrentSite;
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
                {"SiteName", async () => (await _getCurrentSite.GetSite())?.Name},
                {
                    "SiteUrl", async () =>
                    {
                        var site =await _getCurrentSite.GetSite();
                        return (site != null
                            ? $"https://{site.BaseUrl}"
                            : null);
                    }
                },
            };
        }
    }
}