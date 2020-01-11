using Microsoft.AspNetCore.Html;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Widgets;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class GetModelForCurrentPageSubNavigation : GetWidgetModelBase<CurrentPageSubNavigation>
    {
        private readonly IRepository<Webpage> _repository;
        private readonly IGetCurrentPage _getCurrentPage;
        private readonly IGetLiveUrl _getLiveUrl;

        public GetModelForCurrentPageSubNavigation(IRepository<Webpage> repository, IGetCurrentPage getCurrentPage, IGetLiveUrl getLiveUrl)
        {
            _repository = repository;
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
                    {
                        Text = new HtmlString(webpage.Name),
                        Url = new HtmlString(_getLiveUrl.GetUrlSegment(webpage, true)),
                        Children = GetPublishedChildWebpages(webpage.Id)
                            .Select(child =>
                                new NavigationRecord
                                {
                                    Text = new HtmlString(child.Name),
                                    Url = new HtmlString(_getLiveUrl.GetUrlSegment(child, true))
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
            return _repository.Readonly()
                .Where(
                    webpage =>
                        webpage.Parent.Id == parentId && webpage.RevealInNavigation)
                .OrderBy(webpage => webpage.DisplayOrder)
                .ToList()
                .Where(webpage => webpage.Published);
        }
    }
}