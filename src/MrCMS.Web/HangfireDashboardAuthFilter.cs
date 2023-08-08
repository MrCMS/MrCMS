using Hangfire.Dashboard;

namespace MrCMS.Web;

public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        //todo: check if user is admin
        return true;
        //return httpContext.User.IsAdmin();
    }
}