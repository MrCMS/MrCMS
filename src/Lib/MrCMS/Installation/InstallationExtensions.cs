using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Patterns;

namespace MrCMS.Installation
{
    public static class InstallationExtensions
    {
        public static IServiceCollection AddInstallationServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileProvider>(new InstallationContentFileProvider());

            services.AddMvc(options => { }).AddApplicationPart(Assembly.GetAssembly(typeof(InstallationExtensions))).AddRazorRuntimeCompilation(options =>
                {
                    var fileProvider = new InstallationViewFileProvider();
                    options.FileProviders.Insert(0, fileProvider);
                });

            return services;
        }

        public static void ShowInstallation(this IApplicationBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new InstallationContentFileProvider()
            });
            app.UseRouting();
            app.UseEndpoints(builder =>
            {
                builder.MapControllerRoute("Installation", "",
                    new { controller = "Install", action = "Setup" });
                builder.MapFallbackToController("Redirect", "Install"); //.Routes.Add(new InstallationRedirectRouter());
            });
        }

        private class InstallationRedirectRouter : IRouter
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