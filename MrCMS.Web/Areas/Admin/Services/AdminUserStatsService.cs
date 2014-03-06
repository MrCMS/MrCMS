using MrCMS.Entities.People;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class AdminUserStatsService : IAdminUserStatsService
    {
        private readonly ISession _session;

        public AdminUserStatsService(ISession session)
        {
            _session = session;
        }

        public UserStats GetSummary()
        {
            return new UserStats
                   {
                       ActiveUsers = _session.QueryOver<User>().Where(x => x.IsActive).Cacheable().RowCount(),
                       InactiveUsers = _session.QueryOver<User>().WhereNot(x => x.IsActive).Cacheable().RowCount()
                   };
        }
    }
}