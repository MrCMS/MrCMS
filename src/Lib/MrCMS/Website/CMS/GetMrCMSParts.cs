﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Logging;
using MrCMS.Services;
using MrCMS.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
//using Microsoft.OpenApi.Models;

namespace MrCMS.Website.CMS
{
    public class GetMrCMSParts : IGetMrCMSParts
    {
        public IEnumerable<RegistrationInfo> GetSortedMiddleware(MrCMSAppContext appContext,
               Action<IApplicationBuilder> coreFunctions)
        {
            var sortedMiddleware = new List<RegistrationInfo>
            {
                new RegistrationInfo
                {
                    Registration = builder => builder.UseMiddleware<MrCMSLoggingMiddleware>(),
                    Order = int.MaxValue
                },
                new RegistrationInfo
                {
                    Registration = builder => builder.UseMiddleware<CurrentSiteSettingsMiddleware>(),
                    Order = int.MaxValue - 2
                },
                new RegistrationInfo
                {
                    Registration = builder => builder.UseMiddleware<CurrentWebpageMiddleware>(),
                    Order = int.MaxValue - 2
                },
                new RegistrationInfo
                {
                    Registration = coreFunctions,
                    Order = 10000
                },
                new RegistrationInfo
                {
                    Registration = builder => builder.UseMiddleware<RedirectPageMiddleware>(),
                    Order = 9950
                },
                new RegistrationInfo
                {
                    Registration = builder => builder.UseMiddleware<SiteRedirectMiddleware>(),
                    Order = 9850
                },
                new RegistrationInfo
                {
                    Registration = builder =>
                        builder.UseWhen(
                            context => context.RequestServices.GetRequiredService<SiteSettings>().SSLEverywhere,
                            app => app.UseHttpsRedirection()),
                    Order = 9750
                },
                new RegistrationInfo
                {
                    Registration = builder => builder.UseMiddleware<HomePageRedirectMiddleware>(),
                    Order = 9700
                },
                new RegistrationInfo
                {
                    Registration = builder => builder.UseWhen(
                        context => context.RequestServices.GetRequiredService<IGetCurrentUser>().Get()?.IsAdmin == true
                                   && context.RequestServices.GetRequiredService<SiteSettings>().SSLAdmin,
                        app => app.UseHttpsRedirection()),
                    Order = 1040
                },

                new RegistrationInfo
                {
                    Registration = app => app.UseSignalR(routes =>
                    {
                        var methodInfo = typeof(HubRouteBuilder).GetMethod(nameof(HubRouteBuilder.MapHub),
                            new Type[] {typeof(PathString)});
                        var signalRRoutes = appContext.SignalRHubs;
                        foreach (var key in signalRRoutes.Keys)
                        {
                            var pathString = new PathString(signalRRoutes[key]);
                            methodInfo.MakeGenericMethod(key).Invoke(routes, new object[] {pathString});
                        }
                    }),
                    Order = 1001
                },

                //new RegistrationInfo
                //{
                //    Registration = app => app.UseSwagger(),
                //    Order = int.MaxValue - 1
                //},

                //new RegistrationInfo
                //{
                //    Registration = app => app.UseSwaggerUI(c => {
                //        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //    } ),

                //      Order = int.MaxValue - 1
                //},

                new RegistrationInfo
                {
                    Registration = app => app.UseMvc(builder =>
                    {
                        builder.MapMrCMS();
                        builder.MapMrCMSApps(appContext);

                        builder.MapRoute(
                            "default",
                            "{controller=Documentation}/{action=Index}/{id?}");

                        builder.Routes.Add(new FileNotFoundRouter(builder.DefaultHandler));
                    }),
                    Order = 1000
                },
                new RegistrationInfo
                {
                    Registration = builder => builder.UseMiddleware<TrimTrailingSlashMiddleware>(),
                    Order = 501
                },
            };


            sortedMiddleware.AddRange(appContext.Registrations);
            return sortedMiddleware.OrderByDescending(x => x.Order);
        }
    }
}