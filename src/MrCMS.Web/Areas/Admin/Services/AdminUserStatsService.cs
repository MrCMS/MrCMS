using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class AdminUserStatsService : IAdminUserStatsService
    {
        private readonly IGlobalRepository<User> _repository;

        public AdminUserStatsService(IGlobalRepository<User> repository)
        {
            _repository = repository;
        }

        public UserStats GetSummary()
        {
            return new UserStats
            {
                ActiveUsers = _repository.Readonly().Count(x => x.IsActive),
                InactiveUsers = _repository.Readonly().Count(x => !x.IsActive)
            };
        }
    }
}