using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Paging;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public interface IBelongToUserLookupService
    {
        T Get<T>(User user) where T : SystemEntity, IBelongToUser;
        IList<T> GetAll<T>(User user) where T : SystemEntity, IBelongToUser;
        IPagedList<T> GetPaged<T>(User user, QueryOver<T> query = null, int page = 1) where T : SystemEntity, IBelongToUser;
    }
}