using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Search.Models;

namespace MrCMS.Search.ItemCreation
{
    public class GetMediaCategorySearchItem : GetUniversalSearchItemBase<MediaCategory>
    {
        private readonly UrlHelper _urlHelper;
        private readonly IEnumerable<IGetMediaCategorySearchTerms> _getMediaCategorySearchTerms;

        public GetMediaCategorySearchItem(UrlHelper urlHelper, IEnumerable<IGetMediaCategorySearchTerms> getMediaCategorySearchTerms)
        {
            _urlHelper = urlHelper;
            _getMediaCategorySearchTerms = getMediaCategorySearchTerms;
        }

        public override UniversalSearchItem GetSearchItem(MediaCategory mediaCategory)
        {
            return new UniversalSearchItem
            {
                DisplayName = mediaCategory.Name,
                Id = mediaCategory.Id,
                SearchTerms = _getMediaCategorySearchTerms.SelectMany(terms => terms.Get(mediaCategory)),
                SystemType = mediaCategory.GetType().FullName,
                ActionUrl = _urlHelper.Action("Show", "MediaCategory", new { id = mediaCategory.Id, area = "admin" }),
            };
        }
    }
}