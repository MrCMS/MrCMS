using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Website;
using ISession = NHibernate.ISession;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class GetModelForCurrentPageSubNavigation : GetWidgetModelBase<CurrentPageSubNavigation>
    {
        private readonly ISession _session;
        private readonly IGetCurrentPage _getCurrentPage;

        public GetModelForCurrentPageSubNavigation(ISession session, IGetCurrentPage getCurrentPage)
        {
            _session = session;
            _getCurrentPage = getCurrentPage;
        }

        public override object GetModel(CurrentPageSubNavigation widget)
        {
            var currentPage = _getCurrentPage.GetPage();
            var webpages =
                GetPublishedChildWebpages(currentPage.Id);
            var navigationRecords =
                webpages
                    .Select(webpage => new NavigationRecord
                    {
                        Text = new HtmlString(webpage.Name),
                        Url = new HtmlString("/" + webpage.LiveUrlSegment),
                        Children = GetPublishedChildWebpages(webpage.Id)
                            .Select(webpage1 =>
                                new NavigationRecord
                                {
                                    Text = new HtmlString(webpage1.Name),
                                    Url = new HtmlString("/" + webpage1.LiveUrlSegment)
                                }).ToList()
                    }).ToList();

            return new CurrentPageSubNavigationModel
            {
                NavigationList = navigationRecords.ToList(),
                Model = widget
            };
        }
        private IEnumerable<Webpage> GetPublishedChildWebpages(int parentId)
        {
            return _session.QueryOver<Webpage>()
                .Where(
                    webpage =>
                        webpage.Parent.Id == parentId && webpage.RevealInNavigation)
                .OrderBy(webpage => webpage.DisplayOrder).Asc
                .Cacheable()
                .List()
                .Where(webpage => webpage.Published);
        }
    }
}