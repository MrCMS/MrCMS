using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using IdentityServer4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MrCMS.Helpers;
using Microsoft.OpenApi.Models;
using MrCMS.Web.Apps.WebApi.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MrCMS.Web.Apps.WebApi.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterSwagger(this IServiceCollection services)
        {
           
            
            services.AddMvc(c =>
                {
                    c.Conventions.Add(new ApiExplorerMrCMSWebApiOnlyConvention());
                }
               );
           
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options =>
            {
               
                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();

                // integrate xml comments
                //options.IncludeXmlComments(XmlCommentsFilePath);

                // options.SwaggerDoc("v1", new OpenApiInfo{Title = "MrCMS API", Version = "v1"});
                // options.OperationFilter<WebApiOperationFilter>();
                // options.TagActionsBy(api => api.HttpMethod);
            });

           

            return services;

        }

        //static string XmlCommentsFilePath
        //{
        //    get
        //    {
        //        var basePath = PlatformServices.Default.Application.ApplicationBasePath;
        //        var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
        //        return Path.Combine(basePath, fileName);
        //    }
        //}
    }
}
