using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MrCMS.Installation
{
    public static class InstallationExtensions
    {
        public static void ShowInstallation(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMvc(builder =>
            {
                builder.MapRoute("Installation", "",
                    new {controller = "Install", action = "Setup"});
                builder.Routes.Add(new InstallationRedirectRouter());
                // "{controller=Install}/{action=Setup}/{id?}");
            });
        }

        public class InstallationRedirectRouter : IRouter
        {
            public Task RouteAsync(RouteContext context)
            {
                context.Handler = httpContext =>
                {
                    httpContext.Response.Redirect("/");
                    return Task.CompletedTask;
                };
                return Task.CompletedTask;
            }

            public VirtualPathData GetVirtualPath(VirtualPathContext context)
            {
                return null;
            }
        }
    }
}