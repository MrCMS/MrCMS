using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public class GetWebpageSearchItem : GetUniversalSearchItemBase<Webpage>
    {
        private readonly UrlHelper _urlHelper;

        public GetWebpageSearchItem(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override UniversalSearchItem GetSearchItem(Webpage webpage)
        {
            return new UniversalSearchItem
            {
                DisplayName = webpage.Name,
                EditUrl = _urlHelper.Action("Edit", "Webpage", new { id = webpage.Id }),
                Id = webpage.Id,
                SearchTerms = webpage.Tags.Select(tag => tag.Name),
                SystemType = webpage.GetType().FullName,
                ViewUrl = webpage.AbsoluteUrl,
            };
        }
    }
}