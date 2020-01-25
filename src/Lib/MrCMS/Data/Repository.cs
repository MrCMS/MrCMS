using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Common;
using MrCMS.Entities;
using MrCMS.Website;

namespace MrCMS.Data
{
    public class Repository<T> : RepositoryBase<T, IRepository<T>>, IRepository<T> where T : class, IHaveId, IHaveSite
    {
        private readonly IGetSiteId _getSiteId;

        public Repository(IServiceProvider provider) : base(provider)
        {
            _getSiteId = provider.GetRequiredService<IGetSiteId>();
        }

        internal Repository(DbContext context, IGetSiteId getSiteId, ITryGetResult @try,
            ICheckInTransaction checkInTransaction, IGetChangesFromContext getChangesFromContext,
            IHandleDataChanges handleDataChanges, ICheckAllValidators<T> checkAllValidators,
            IApplyPrePersistence<T> applyPrePersistence) : base(context, @try, checkInTransaction,
            getChangesFromContext, handleDataChanges, checkAllValidators, applyPrePersistence)
        {
            _getSiteId = getSiteId;
        }

        public override IQueryable<T> Query()
        {
            var siteId = _getSiteId.GetId();
            return base.Query().Where(x => x.SiteId == siteId).Include(x => x.Site);
        }

        protected override void BeforeAdd(T entity)
        {
            entity.SiteId = _getSiteId.GetId();
        }

        protected override IRepository<T> RepoInstance => this;
    }
}