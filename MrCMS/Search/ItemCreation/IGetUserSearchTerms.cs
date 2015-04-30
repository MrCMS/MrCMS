using System.Collections.Generic;
using MrCMS.Entities.People;

namespace MrCMS.Search.ItemCreation
{
    public interface IGetUserSearchTerms
    {
        IEnumerable<string> GetPrimary(User user);
        Dictionary<User,HashSet<string>> GetPrimary(HashSet<User> users);
        IEnumerable<string> GetSecondary(User user);
        Dictionary<User,HashSet<string>> GetSecondary(HashSet<User> users);
    }
}