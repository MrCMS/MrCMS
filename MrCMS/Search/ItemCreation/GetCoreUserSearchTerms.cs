using System.Collections.Generic;
using MrCMS.Entities.People;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreUserSearchTerms : IGetUserSearchTerms
    {
        public IEnumerable<string> Get(User user)
        {
            yield return user.Email;
            if (!string.IsNullOrWhiteSpace(user.FirstName))
                yield return user.FirstName;
            if (!string.IsNullOrWhiteSpace(user.LastName))
                yield return user.LastName;
        }
    }
}