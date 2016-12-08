using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class BelongToUserLookupService : IBelongToUserLookupService
    {
        private readonly ISession _session;

        public BelongToUserLookupService(ISession session)
        {
            _session = session;
        }

        public T Get<T>(User user) where T : SystemEntity, IBelongToUser
        {
            return _session.QueryOver<T>().Where(arg => arg.User == user).Take(1).Cacheable().SingleOrDefault();
        }

        public IList<T> GetAll<T>(User user) where T : SystemEntity, IBelongToUser
        {
            return _session.QueryOver<T>().Where(arg => arg.User == user).Cacheable().List();
        }

        public IPagedList<T> GetPaged<T>(User user, QueryOver<T> query = null, int page = 1)
            where T : SystemEntity, IBelongToUser
        {
            return _session.Paged(query ?? QueryOver.Of<T>(), page);
        }

    }
}