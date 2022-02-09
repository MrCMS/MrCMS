using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IAdminUserStatsService
    {
        UserStats GetSummary();
    }
}