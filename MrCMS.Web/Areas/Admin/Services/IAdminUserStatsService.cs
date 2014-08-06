using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IAdminUserStatsService
    {
        UserStats GetSummary();
    }
}