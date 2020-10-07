using Microsoft.AspNetCore.Html;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Widgets;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using ISession = NHibernate.ISession;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class GetModelForCurrentPageSubNavigation : GetWidgetModelBase<CurrentPageSubNavigation>
    {
        private readonly ISession _session;
        private readonly IGetCurrentPage _getCurrentPage;
        private readonly IGetLiveUrl _getLiveUrl;

        public GetModelForCurrentPageSubNavigation(ISession session, IGetCurrentPage getCurrentPage,
            IGetLiveUrl getLiveUrl)
        {
            _session = session;
            _getCurrentPage = getCurrentPage;
            _getLiveUrl = getLiveUrl;
        }

        public override object GetModel(CurrentPageSubNavigation widget)
        {
            var currentPage = _getCurrentPage.GetPage();
            var webpages =
                GetPublishedChildWebpages(currentPage.Id);
            var navigationRecords =
                webpages
                    .Select(webpage => new NavigationRecord
                    (webpage.Name, _getLiveUrl.GetUrlSegment(webpage, true),
                        webpage.Unproxy().GetType(), GetPublishedChildWebpages(webpage.Id)
                            .Select(child =>
                                new NavigationRecord(child.Name, _getLiveUrl.GetUrlSegment(child, true),
                                    child.Unproxy().GetType()))
                    )).ToList();

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