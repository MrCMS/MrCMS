using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Search.Models;

namespace MrCMS.Search.ItemCreation
{
    public class GetUserSearchItem : GetUniversalSearchItemBase<User>
    {
        private readonly UrlHelper _urlHelper;
        private readonly IEnumerable<IGetUserSearchTerms> _userSearchTerms;

        public GetUserSearchItem(UrlHelper urlHelper, IEnumerable<IGetUserSearchTerms> userSearchTerms)
        {
            _urlHelper = urlHelper;
            _userSearchTerms = userSearchTerms;
        }

        public override UniversalSearchItem GetSearchItem(User mediaCategory)
        {
            return new UniversalSearchItem
            {
                DisplayName = mediaCategory.Name,
                EditUrl = _urlHelper.Action("Edit", "User", new { id = mediaCategory.Id }),
                Id = mediaCategory.Id,
                SearchTerms = _userSearchTerms.SelectMany(terms => terms.Get(mediaCategory)),
                SystemType = mediaCategory.GetType().FullName
            };
        }
    }
}