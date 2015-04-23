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
            HashSet<string> primarySearchTerms = _userSearchTerms.SelectMany(terms => terms.GetPrimary(entity)).ToHashSet();
            HashSet<string> secondarySearchTerms = _userSearchTerms.SelectMany(terms => terms.GetSecondary(entity)).ToHashSet();
            return GetUniversalSearchItem(entity, primarySearchTerms, secondarySearchTerms);
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<User> entities)
        {
            var primaryDictionary = new Dictionary<User, HashSet<string>>();
            foreach (IGetUserSearchTerms userSearchTerms in _userSearchTerms)
            {
                Dictionary<User, HashSet<string>> terms = userSearchTerms.GetPrimary(entities);
                foreach (User key in terms.Keys)
                {
                    if (primaryDictionary.ContainsKey(key))
                    {
                        primaryDictionary[key].AddRange(terms[key]);
                    }
                    primaryDictionary[key] = terms[key];
                }
            }
            var secondaryDictionary = new Dictionary<User, HashSet<string>>();
            foreach (IGetUserSearchTerms userSearchTerms in _userSearchTerms)
            {
                Dictionary<User, HashSet<string>> terms = userSearchTerms.GetSecondary(entities);
                foreach (User key in terms.Keys)
                {
                    if (secondaryDictionary.ContainsKey(key))
                    {
                        secondaryDictionary[key].AddRange(terms[key]);
                    }
                    secondaryDictionary[key] = terms[key];
                }
            }

            return
                entities.Select(user =>
                    GetUniversalSearchItem(user,
                        primaryDictionary.ContainsKey(user) ? primaryDictionary[user] : new HashSet<string>(),
                        secondaryDictionary.ContainsKey(user) ? secondaryDictionary[user] : new HashSet<string>()
                        )).ToHashSet();
        }

        private UniversalSearchItem GetUniversalSearchItem(User entity, HashSet<string> primarySearchTerms, HashSet<string>  secondarySearchTerms)
        {
            return new UniversalSearchItem
            {
                DisplayName = entity.Name,
                ActionUrl = "/admin/user/edit/" + entity.Id,
                Id = entity.Id,
                PrimarySearchTerms = primarySearchTerms,
                SecondarySearchTerms = secondarySearchTerms,
                SystemType = entity.GetType().FullName,
                CreatedOn = entity.CreatedOn,
                SearchGuid = entity.Guid
            };
        }
    }
}