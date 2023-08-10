using System.Threading.Tasks;
using Hangfire.Dashboard;
using MrCMS.Entities.People;
using MrCMS.Services;

namespace MrCMS.Web;

public class HangfireDashboardAuthFilter : IDashboardAsyncAuthorizationFilter
{
    private readonly IGetCurrentUser _getCurrentUser;

    public HangfireDashboardAuthFilter(IGetCurrentUser getCurrentUser)
    {
        _getCurrentUser = getCurrentUser;
    }
    public async Task<bool> AuthorizeAsync(DashboardContext context)
    {
        return context.GetHttpContext().User.IsInRole(UserRole.Administrator);
    }
}