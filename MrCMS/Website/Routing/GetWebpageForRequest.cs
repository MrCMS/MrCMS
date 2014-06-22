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
        private readonly IDocumentService _documentService;

        public GetWebpageForRequest(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        private Dictionary<string, Webpage> _cache = new Dictionary<string, Webpage>();

        public Webpage Get(RequestContext context)
        {
            string data = Convert.ToString(context.RouteData.Values["data"]);

            if (_cache.ContainsKey(data))
                return _cache[data];

            Webpage webpage = string.IsNullOrWhiteSpace(data)
                ? CurrentRequestData.HomePage
                : _documentService.GetDocumentByUrl<Webpage>(data);

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