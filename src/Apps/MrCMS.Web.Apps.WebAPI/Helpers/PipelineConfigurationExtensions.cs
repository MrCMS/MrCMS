using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Website.CMS;

namespace MrCMS.Web.Apps.WebApi.Helpers
{
    public static class PipelineConfigurationExtensions
    {
        public static IApplicationBuilder UseMrCMSSwagger(this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mr CMS API V1");
            });

            app.UseMvc();

            return app;
        }
    }
}