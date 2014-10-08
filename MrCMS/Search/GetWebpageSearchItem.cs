using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public class GetWebpageSearchItem : GetUniversalSearchItemBase<Webpage>
    {
        private readonly UrlHelper _urlHelper;
        private readonly IEnumerable<IGetWebpageSearchTerms> _getWebpageSearchTerms;

        public GetWebpageSearchItem(UrlHelper urlHelper, IEnumerable<IGetWebpageSearchTerms> getWebpageSearchTerms)
        {
            _urlHelper = urlHelper;
            _getWebpageSearchTerms = getWebpageSearchTerms;
        }

        public override UniversalSearchItem GetSearchItem(Webpage mediaCategory)
        {
            return new UniversalSearchItem
            {
                DisplayName = mediaCategory.Name,
                EditUrl = _urlHelper.Action("Edit", "Webpage", new {id = mediaCategory.Id}),
                Id = mediaCategory.Id,
                SearchTerms = _getWebpageSearchTerms.SelectMany(terms => terms.Get(mediaCategory)),
                SystemType = mediaCategory.GetType().FullName,
                ViewUrl = mediaCategory.AbsoluteUrl,
            };
        }
    }
}