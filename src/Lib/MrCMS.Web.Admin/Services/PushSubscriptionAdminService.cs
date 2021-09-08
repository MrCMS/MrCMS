using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using NHibernate;
using NHibernate.Linq;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class PushSubscriptionAdminService : IPushSubscriptionAdminService
    {
        private readonly ISession _session;

        public PushSubscriptionAdminService(ISession session)
        {
            _session = session;
        }

        public async Task<IPagedList<PushSubscription>> Search(PushSubscriptionSearchQuery searchQuery)
        {
            IQueryable<PushSubscription> query = _session.Query<PushSubscription>().OrderByDescending(x => x.CreatedOn);

            if (!string.IsNullOrWhiteSpace(searchQuery.Email))
                query = query.Where(x => x.User != null && x.User.Email.Like($"%{searchQuery.Email}%"));

            return await query.PagedAsync(searchQuery.Page);
        }
    }
}