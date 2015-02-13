using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Search.Models;

namespace MrCMS.Search.ItemCreation
{
    public class GetUserSearchItem : GetUniversalSearchItemBase<User>
    {
        private readonly IEnumerable<IGetUserSearchTerms> _userSearchTerms;

        public GetUserSearchItem(IEnumerable<IGetUserSearchTerms> userSearchTerms)
        {
            _userSearchTerms = userSearchTerms;
        }

        public override UniversalSearchItem GetSearchItem(User entity)
        {
            HashSet<string> searchTerms = _userSearchTerms.SelectMany(terms => terms.Get(entity)).ToHashSet();
            return GetUniversalSearchItem(entity, searchTerms);
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<User> entities)
        {
            var dictionary = new Dictionary<User, HashSet<string>>();
            foreach (IGetUserSearchTerms userSearchTerms in _userSearchTerms)
            {
                Dictionary<User, HashSet<string>> terms = userSearchTerms.Get(entities);
                foreach (User key in terms.Keys)
                {
                    if (dictionary.ContainsKey(key))
                    {
                        dictionary[key].AddRange(terms[key]);
                    }
                    dictionary[key] = terms[key];
                }
            }

            return
                entities.Select(user =>
                    GetUniversalSearchItem(user,
                        dictionary.ContainsKey(user) ? dictionary[user] : new HashSet<string>())).ToHashSet();
        }

        private UniversalSearchItem GetUniversalSearchItem(User entity, HashSet<string> searchTerms)
        {
            return new UniversalSearchItem
            {
                DisplayName = entity.Name,
                ActionUrl = "/admin/user/edit/" + entity.Id, //_urlHelper.Action("Edit", "User", new {id = entity.Id, area = "admin"}),
                Id = entity.Id,
                SearchTerms = searchTerms,
                SystemType = entity.GetType().FullName
            };
        }
    }
}