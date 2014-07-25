using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class GetModelForCurrentPageSubNavigation : GetWidgetModelBase<CurrentPageSubNavigation>
    {
        private readonly ISession _session;

        public GetModelForCurrentPageSubNavigation(ISession session)
        {
            _session = session;
        }

        public override object GetModel(CurrentPageSubNavigation widget)
        {
            var webpages =
                GetPublishedChildWebpages(CurrentRequestData.CurrentPage.Id);
            var navigationRecords =
                webpages
                    .Select(webpage => new NavigationRecord
                    {
                        Text = MvcHtmlString.Create(webpage.Name),
                        Url = MvcHtmlString.Create("/" + webpage.LiveUrlSegment),
                        Children = GetPublishedChildWebpages(webpage.Id)
                            .Select(webpage1 =>
                                new NavigationRecord
                                {
                                    Text = MvcHtmlString.Create(webpage1.Name),
                                    Url = MvcHtmlString.Create("/" + webpage1.LiveUrlSegment)
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