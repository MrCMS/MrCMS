using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MrCMS.Helpers;
using Microsoft.OpenApi.Models;
using MrCMS.Web.Apps.WebApi.Filters;

namespace MrCMS.Web.Apps.WebApi.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterSwagger(this IServiceCollection services)
        {
            // serviceCollection.addmv

            services.AddMvc(c =>
                {
                    c.Conventions.Add(new ApiExplorerMrCMSWebApiOnlyConvention());
                }
               );

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo{Title = "MrCMS API", Version = "v1"});
               // options.OperationFilter<WebApiOperationFilter>();
               // options.TagActionsBy(api => api.HttpMethod);
            });

            //var builder = services.AddIdentityServer()
            //    .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
            //    .AddInMemoryApiResources(IdentityServerConfig.GetApis())
            //    .AddInMemoryClients(IdentityServerConfig.GetClients())
            //    .AddTestUsers(IdentityServerConfig.GetUsers());

            //IServiceProvider serviceProvider = services.BuildServiceProvider();
            //IHostingEnvironment env = serviceProvider.GetService<IHostingEnvironment>();

            //if (env.IsDevelopment())
            //{
            //    builder.AddDeveloperSigningCredential();
            //}
            //else
            //{
            //    throw new Exception("need to configure key material");
            //}

            //services.AddAuthentication()
            //    .AddGoogle("Google", options =>
            //    {
            //        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            //        options.ClientId = "158523382455-h9njt5fd2a04ruascm0d52dj9amtcvcp.apps.googleusercontent.com";
            //        options.ClientSecret = "lmuz8Nr1B9-H4ExJEmL7zwbq";
            //    })
            //    .AddOpenIdConnect("oidc", "OpenID Connect", options =>
            //    {
            //        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //        options.SignOutScheme = IdentityServerConstants.SignoutScheme;
            //        options.SaveTokens = true;

            //        options.Authority = "https://demo.identityserver.io/";
            //        options.ClientId = "implicit";

            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            NameClaimType = "name",
            //            RoleClaimType = "role"
            //        };
            //    });

            return services;

        }
    }
}
