using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Widgets;
using X.PagedList;

namespace MrCMS.Web.Apps.Core.Services.Widgets
{
    public class GetNavigationRecords : GetWidgetModelBase<Navigation>
    {
        private readonly IRepository<Webpage> _repository;
        private readonly IGetLiveUrl _getLiveUrl;

        public GetNavigationRecords(IRepository<Webpage> repository, IGetLiveUrl getLiveUrl)
        {
            _repository = repository;
            _getLiveUrl = getLiveUrl;
        }

        public override async Task<object> GetModel(Navigation widget)
        {
            var rootPages =await GetPages(null);
            var childPages = widget.IncludeChildren ? await GetPages(rootPages) : new List<Webpage>();
            var navigationRecords =
                rootPages.Where(webpage => webpage.Published).OrderBy(webpage => webpage.DisplayOrder)
                       .Select(webpage => new NavigationRecord
                       {
                           Text = new HtmlString(webpage.Name),
                           Url = new HtmlString(_getLiveUrl.GetUrlSegment(webpage, true)),
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

        private Task<List<Webpage>> GetPages(IList<Webpage> parents)
        {
            var queryOver = _repository.Query();
            if (parents == null)
            {
                queryOver = queryOver.Where(webpage => webpage.ParentId == null);
            }
            else
            {
                var parentIds = parents.Select(p => (int?)p.Id).ToList();
                queryOver = queryOver.Where(webpage => parentIds.Contains(webpage.ParentId));
            }
            return
                queryOver.Where(webpage => webpage.RevealInNavigation && webpage.Published)
                    .OrderBy(webpage => webpage.DisplayOrder)
                    //.Cacheable()
                    .ToListAsync();

        }
    }
}