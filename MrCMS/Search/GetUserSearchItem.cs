using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public class GetUserSearchItem : GetUniversalSearchItemBase<User>
    {
        private readonly UrlHelper _urlHelper;

        public GetUserSearchItem(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override UniversalSearchItem GetSearchItem(User user)
        {
            return new UniversalSearchItem
            {
                DisplayName = user.Name,
                EditUrl = _urlHelper.Action("Edit", "User", new { id = user.Id }),
                Id = user.Id,
                SearchTerms = GetSearchTerms(user),
                SystemType = user.GetType().FullName
            };
        }

        private IEnumerable<string> GetSearchTerms(User user)
        {
            yield return user.Email;
            if (!string.IsNullOrWhiteSpace(user.FirstName))
                yield return user.FirstName;
            if (!string.IsNullOrWhiteSpace(user.LastName))
                yield return user.LastName;
        }
    }
}