using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IBelongToUserLookupService
    {
        T Get<T>(User user) where T : SystemEntity, IBelongToUser;
        IList<T> GetAll<T>(User user) where T : SystemEntity, IBelongToUser;
    }
}