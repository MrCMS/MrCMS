using System;
using System.Collections.Generic;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.Routing
{
    public class GetWebpageForRequest : IGetWebpageForRequest
    {
        private readonly IGetDocumentByUrl<Webpage> _getWebpageByUrl;

        public GetWebpageForRequest(IGetDocumentByUrl<Webpage> getWebpageByUrl)
        {
            _getWebpageByUrl = getWebpageByUrl;
        }

        private Dictionary<string, Webpage> _cache = new Dictionary<string, Webpage>();

        public Webpage Get(RequestContext context, string url = null)
        {
            string data = url ?? Convert.ToString(context.RouteData.Values["data"]);

            if (_cache.ContainsKey(data))
                return _cache[data];

            Webpage webpage = string.IsNullOrWhiteSpace(data)
                ? CurrentRequestData.HomePage
                : _getWebpageByUrl.GetByUrl(data);

            if (webpage != null && !webpage.Published && !CurrentRequestData.CurrentUserIsAdmin)
            {
                webpage = null;
            }
            CurrentRequestData.CurrentPage = webpage;

            if (webpage != null)
            {
                if (MrCMSApp.AppWebpages.ContainsKey(webpage.GetType()))
                    context.RouteData.DataTokens["app"] = MrCMSApp.AppWebpages[webpage.GetType()];
            }

            _cache[data] = webpage;
            return webpage;
        }
    }
}