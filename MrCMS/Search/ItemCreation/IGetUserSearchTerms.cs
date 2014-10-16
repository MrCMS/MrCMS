using System.Collections.Generic;
using MrCMS.Entities.People;

namespace MrCMS.Search.ItemCreation
{
    public interface IGetUserSearchTerms
    {
        IEnumerable<string> Get(User user);
    }
}