using System.Collections.Generic;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IAdminPageStatsService
    {
        IList<WebpageStats> GetSummary();
    }
}