using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Website.CMS;

namespace MrCMS.Web.Apps.WebApi.Helpers
{
    public static class PipelineConfigurationExtensions
    {
        public static IApplicationBuilder UseMrCMSSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
           
            // Enable middleware to serve generated Swagger as a JSON endpoint.
        
            app.UseSwagger();

            //// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            //// specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(options =>
            {
                //  options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mr CMS API V1");
                if (provider != null)
                { 
                    /// This is always causing error during startup.
                    //foreach (var description in provider.ApiVersionDescriptions)
                    //{
                    //    //options.SwaggerEndpoint(
                    //    //    $"/swagger/{description.GroupName}/swagger.json",
                    //    //    description.GroupName.ToUpperInvariant());
                    //}
                    
                    //A Temporary way sort out version selection in Swagger till solution to above code is found
                    for (int i = 1; i < 3; i++)
                    {
                        options.SwaggerEndpoint(
                                $"/swagger/v{i}/swagger.json",
                                $"V{i}".ToUpperInvariant());
                    }
                }
                else
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mr CMS API V1");
                }

            });

            return app;
        }
    }
}