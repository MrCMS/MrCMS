using System;
using System.Linq;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using NHibernate;

namespace MrCMS.Website.Routing
{
    public class GetWebpageByUrlHistoryForRequest : IGetWebpageByUrlHistoryForRequest
    {
        private readonly ISession _session;

        public GetWebpageByUrlHistoryForRequest(ISession session)
        {
            _session = session;
        }

        public Webpage Get(RequestContext context)
        {
            string data = Convert.ToString(context.RouteData.Values["data"]);
            UrlHistory historyItemByUrl = GetHistoryItemByUrl(data);
            if (historyItemByUrl != null)
            {
                return historyItemByUrl.Webpage;
            }
            if (context.HttpContext.Request.Url != null)
            {
                UrlHistory historyItemByUrlContent =
                    GetHistoryItemByUrl(context.HttpContext.Request.Url.PathAndQuery.TrimStart('/'));
                if (historyItemByUrlContent != null)
                {
                    return historyItemByUrlContent.Webpage;
                }
            }
            return null;
        }

        private UrlHistory GetHistoryItemByUrl(string url)
        {
            return _session.QueryOver<UrlHistory>()
                .Where(doc => doc.UrlSegment == url)
                .Cacheable().List().FirstOrDefault();
        }
    }
}