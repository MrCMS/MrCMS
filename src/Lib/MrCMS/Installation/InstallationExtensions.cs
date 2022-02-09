using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Installation
{
    public static class InstallationExtensions
    {
        public static IServiceCollection AddInstallationServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileProvider>(new InstallationContentFileProvider());
            services.AddScoped<ICurrentSiteLocator>(provider =>
            {
                var contextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                var context = contextAccessor.HttpContext;
                var site = context.Items["override-site"] as Site;
                return new KnownSiteLocator(site);
            });

            services.AddMvc();

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