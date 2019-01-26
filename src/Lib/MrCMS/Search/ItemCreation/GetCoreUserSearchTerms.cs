using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Helpers;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreUserSearchTerms : IGetUserSearchTerms
    {
        public IEnumerable<string> GetPrimary(User user)
        {
            yield return user.Email;
            if (!string.IsNullOrWhiteSpace(user.FirstName))
                yield return user.FirstName;
            if (!string.IsNullOrWhiteSpace(user.LastName))
                yield return user.LastName;
        }

        public Dictionary<User, HashSet<string>> GetPrimary(HashSet<User> users)
        {
            return users.ToDictionary(user => user, user => GetPrimary(user).ToHashSet());
        }

        public IEnumerable<string> GetSecondary(User user)
        {
            yield break;
        }

        public Dictionary<User, HashSet<string>> GetSecondary(HashSet<User> users)
        {
            return users.ToDictionary(user => user, user => GetSecondary(user).ToHashSet());
        }
    }
}