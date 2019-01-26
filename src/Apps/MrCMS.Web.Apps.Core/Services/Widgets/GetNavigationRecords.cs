using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Widgets;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class GetNavigationRecords : GetWidgetModelBase<Navigation>
    {
        private readonly ISession _session;
        private readonly IGetLiveUrl _getLiveUrl;

        public GetNavigationRecords(ISession session, IGetLiveUrl getLiveUrl)
        {
            _session = session;
            _getLiveUrl = getLiveUrl;
        }

        public override object GetModel(Navigation widget)
        {
            var rootPages = GetPages(null);
            var childPages = widget.IncludeChildren ? GetPages(rootPages) : new List<Webpage>();
            var navigationRecords =
                rootPages.Where(webpage => webpage.Published).OrderBy(webpage => webpage.DisplayOrder)
                       .Select(webpage => new NavigationRecord
                       {
                           Text = new HtmlString(webpage.Name),
                           Url = new HtmlString(_getLiveUrl.GetUrlSegment(webpage,true)),
                           Children = childPages.Where(webpage1 => webpage1.ParentId == webpage.Id)
                                            .Select(child =>
                                                    new NavigationRecord
                                                    {
                                                        Text = new HtmlString(child.Name),
                                                        Url = new HtmlString(_getLiveUrl.GetUrlSegment(child, true))
                                                    }).ToList()
                       }).ToList();

            return new NavigationList(navigationRecords.ToList());
        }

        private IList<Webpage> GetPages(IList<Webpage> parents)
        {
            var queryOver = _session.QueryOver<Webpage>();
            if (parents == null)
            {
                queryOver = queryOver.Where(webpage => webpage.Parent == null);
            }
            else
            {
                var parentIds = parents.Select(p => p.Id).ToList();
                queryOver = queryOver.Where(webpage => webpage.Parent.Id.IsIn(parentIds));
            }
            return
                queryOver.Where(webpage => webpage.RevealInNavigation)
                    .OrderBy(webpage => webpage.DisplayOrder).Asc
                    .Cacheable()
                    .List().ToList(x => x.Published);
        }
    }
}