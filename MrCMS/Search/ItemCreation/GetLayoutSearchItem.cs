using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Search.Models;

namespace MrCMS.Search.ItemCreation
{
    public class GetLayoutSearchItem : GetUniversalSearchItemBase<Layout>
    {
        private readonly UrlHelper _urlHelper;
        private readonly IEnumerable<IGetLayoutSearchTerms> _getLayoutSearchTerms;

        public GetLayoutSearchItem(UrlHelper urlHelper, IEnumerable<IGetLayoutSearchTerms> getLayoutSearchTerms)
        {
            _urlHelper = urlHelper;
            _getLayoutSearchTerms = getLayoutSearchTerms;
        }

        public override UniversalSearchItem GetSearchItem(Layout mediaCategory)
        {
            return new UniversalSearchItem
            {
                DisplayName = mediaCategory.Name,
                ActionUrl = _urlHelper.Action("Edit", "Layout", new { id = mediaCategory.Id, area = "admin" }),
                Id = mediaCategory.Id,
                SearchTerms = _getLayoutSearchTerms.SelectMany(terms => terms.Get(mediaCategory)),
                SystemType = mediaCategory.GetType().FullName
            };
        }
    }
}