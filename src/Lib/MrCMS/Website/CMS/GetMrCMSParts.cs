using Microsoft.AspNetCore.Builder;
using MrCMS.Apps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class GetMrCMSParts : IGetMrCMSParts
    {
        public IEnumerable<ApplicationRegistrationInfo> GetSortedMiddleware(MrCMSAppContext appContext,
            Action<IApplicationBuilder> coreFunctions)
        {
            var sortedMiddleware = new List<ApplicationRegistrationInfo>
            {
                new()
                {
                    Registration = coreFunctions,
                    Order = int.MaxValue
                },
                new()
                {
                    Registration = builder => builder.UseMiddleware<SetCurrentSite>(),
                    Order = int.MaxValue - 1
                },

                // new()
                // {
                //     Registration = builder =>
                //         builder.UseWhen(
                //             context => !Path.HasExtension(context.Request.Path),
                //             app => app.UseMiddleware<CurrentWebpageMiddleware>()),
                //     Order = int.MaxValue - 2
                // },
                //
                // new()
                // {
                //     Registration = builder => builder.UseMiddleware<RedirectPageMiddleware>(),
                //     Order = 9950
                // },
                // new()
                // {
                //     Registration = builder =>
                //         builder.UseWhen(
                //             context => !Path.HasExtension(context.Request.Path),
                //             app => app.UseMiddleware<SiteRedirectMiddleware>()),
                //     Order = 9850
                // },
                // new()
                // {
                //     Registration = builder =>
                //         builder.UseWhen(
                //             context => context.Request.Path == "/",
                //             app => app.UseMiddleware<HomePageRedirectMiddleware>()),
                //     Order = 9700
                // },
                // new()
                // {
                //     Registration = builder => builder.UseMiddleware<TrimTrailingSlashMiddleware>(),
                //     Order = 501
                // },
                // new()
                // {
                //     Registration = builder => builder.UseMiddleware<UrlHistoryMiddleware>(),
                //     Order = 100
                // },
            };


            sortedMiddleware.AddRange(appContext.Registrations);
            return sortedMiddleware.OrderByDescending(x => x.Order);
        }

        public class SetCurrentSite : IMiddleware
        {
            public async Task InvokeAsync(HttpContext context, RequestDelegate next)
            {
                var provider = context.RequestServices;
                var site =
                    provider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Items["override-site"] as Site;

                var siteLocator = provider.GetRequiredService<ICurrentSiteLocator>();
                site ??= siteLocator.GetCurrentSite().Unproxy();
                if (site == null)
                {
                    // check for redirected domain
                    var domain = siteLocator.GetCurrentRedirectedDomain().Unproxy();
                    var redirectSite = domain?.Site.Unproxy();
                    if (redirectSite == null)
                    {
                        context.Response.StatusCode = 404;
                        return;
                    }

                    context.Response.Redirect(
                        $"https://{redirectSite.BaseUrl.Trim('/')}{context.Request.Path}{context.Request.QueryString}",
                        true);
                    return;
                }

                var session = provider.GetRequiredService<NHibernate.ISession>();
                if (site != null)
                {
                    session.EnableFilter("SiteFilter").SetParameter("site", site.Id);
                    context.Items["current-site"] = site;
                }

                await next(context);
            }
        }

        public IEnumerable<EndpointRegistrationInfo> GetSortedEndpoints(MrCMSAppContext appContext,
            Action<IEndpointRouteBuilder> coreFunctions)
        {
            var list = new List<EndpointRegistrationInfo>
            {
                new()
                {
                    Registration = coreFunctions,
                    Order = 10000
                },
                new()
                {
                    Registration = builder =>
                    {
                        builder.MapMrCMS(appContext);

                        builder.MapControllerRoute(
                            "default",
                            "{controller=Home}/{action=Index}/{id?}");

                        //builder.Routes.Add(new FileNotFoundRouter(builder.DefaultHandler));
                    },
                    Order = 1000
                },
            };


            list.AddRange(appContext.EndpointRegistrations);
            return list.OrderByDescending(x => x.Order);
        }
    }
}
