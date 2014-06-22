using System;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.Routing
{
    public class GetWebpageByUrlHistoryForRequest : IGetWebpageByUrlHistoryForRequest
    {
        private readonly IDocumentService _documentService;

        public GetWebpageByUrlHistoryForRequest(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public Webpage Get(RequestContext context)
        {
            string data = Convert.ToString(context.RouteData.Values["data"]);
            var historyItemByUrl = _documentService.GetHistoryItemByUrl(data);
            if (historyItemByUrl != null)
            {
                return historyItemByUrl.Webpage;
            }
            if (context.HttpContext.Request.Url != null)
            {
                var historyItemByUrlContent = _documentService.GetHistoryItemByUrl(context.HttpContext.Request.Url.PathAndQuery.TrimStart('/'));
                if (historyItemByUrlContent != null)
                {
                    return historyItemByUrlContent.Webpage;
                }
            }
            return null;
        }
    }
}