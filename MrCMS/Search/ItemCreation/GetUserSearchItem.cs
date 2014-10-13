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

        public override UniversalSearchItem GetSearchItem(User user)
        {
            return new UniversalSearchItem
            {
                DisplayName = user.Name,
                ActionUrl = _urlHelper.Action("Edit", "User", new { id = user.Id, area="admin" }),
                Id = user.Id,
                SearchTerms = _userSearchTerms.SelectMany(terms => terms.Get(user)),
                SystemType = user.GetType().FullName
            };
        }
    }
}