using System.Collections.Generic;
using MrCMS.Entities.People;

namespace MrCMS.Search
{
    public interface IGetUserSearchTerms
    {
        IEnumerable<string> Get(User user);
    }
}