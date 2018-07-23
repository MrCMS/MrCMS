using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website.CMS;
using NHibernate;
using ISession = NHibernate.ISession;

namespace MrCMS.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterAllSimplePairings();
            services.RegisterOpenGenerics();
            services.SelfRegisterAllConcreteTypes();

            services.AddMvc(options =>
            {

            });
            services.AddSingleton<ISessionFactory>(provider => new NHibernateConfigurator(new SqlServer2012Provider(
                new DatabaseSettings
                {
                    ConnectionString =
                        "Data Source=localhost;Initial Catalog=mrcms-migration;Integrated Security=True;Persist Security Info=False;MultipleActiveResultSets=True"
                })).CreateSessionFactory());
            services.AddScoped<ISession>(provider => provider.GetRequiredService<ISessionFactory>().OpenSession());

            services.AddSingleton<ICmsMethodTester, CmsMethodTester>();
            services.AddSingleton<IAssignPageDataToRouteData, AssignPageDataToRouteData>();
            services.AddSingleton<IQuerySerializer, QuerySerializer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(builder =>
            {
                builder.MapMrCMS();

                builder.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
