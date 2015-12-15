using System;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using NHibernate;
using NHibernate.Proxy;

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
            if (historyItemByUrl != null && historyItemByUrl.Webpage.Published)
            {
                return historyItemByUrl.Webpage;
            }
            if (context.HttpContext.Request.Url != null)
            {
                UrlHistory historyItemByUrlContent =
                    GetHistoryItemByUrl(context.HttpContext.Request.Url.PathAndQuery.TrimStart('/'));
                if (historyItemByUrlContent != null && historyItemByUrlContent.Webpage.Published)
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
                .Take(1).Cacheable()
                .SingleOrDefault();
        }
    }
}