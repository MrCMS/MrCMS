// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.IdentityServer.NHibernate.Storage;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Helpers;
using Client = IdentityServer4.Models.Client;

namespace MrCMS.Web.IdentityServer
{
    public class Startup
    {
        private const string Database = nameof(Database);
        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        private readonly ILogger _logger;

        private readonly IOptions<DatabaseSettings> _databaseSettings;
        public Startup(IConfiguration configuration, IHostingEnvironment environment, ILogger<Startup> logger)
        {
            Environment = environment;
            Configuration = configuration;
            _logger = logger;

            TypeHelper.Initialize(GetType().Assembly);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var isInstalled = IsInstalled();

            services.RegisterAllSimplePairings();
            services.RegisterOpenGenerics();
            services.SelfRegisterAllConcreteTypes();
            services.AddSession();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // uncomment, if you want to add an MVC-based UI
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            var appContext = services.AddMrCMSApps(context =>
            {

                context.RegisterApp<MrCMSIs4App>();
            });

            // services.AddMrCMSDataAccess(isInstalled, Configuration.GetSection(Database));
            services.AddMrCMSDataAccessToIS4(isInstalled, Configuration.GetSection(Database));

        services.AddAutoMapper(expression =>
        {
            expression.AllowNullDestinationValues = true;
            appContext.ConfigureAutomapper(expression);
        });

            //if (isInstalled)
            //{
            //    services.Configure<DatabaseSettings>(Configuration.GetSection(Database));
            // }

            var builder = services.AddIdentityServer();
                //.AddInMemoryIdentityResources(Config.GetIdentityResources())
                //.AddInMemoryApiResources(Config.GetApis())
                //.AddInMemoryClients(Config.GetClients())
                //.AddTestUsers(Config.GetUsers());

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }

           // var dbsettings = Configuration.GetSection("Database").Get<DatabaseSettings>();

            //var nnn = GetType().Assembly.FullName;
             //_logger.LogInformation($"Added Todo Repository to services {nnn}", nnn);



             builder.AddNHibernateStores();
            builder.AddMrCMSUserStore();
           
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to support static files
            app.UseStaticFiles();

            app.UseIdentityServer();

            // uncomment, if you want to add an MVC-based UI
            app.UseMvcWithDefaultRoute();
        }

        private bool IsInstalled()
        {
            var dbSection = Configuration.GetSection(Database);
            return dbSection.Exists();

       // IdentityServer4.NHibernate.Entities.Client
        }
    }
}
