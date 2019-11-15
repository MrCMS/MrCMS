using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MrCMS.Services.Resources;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MrCMS.Web.Apps.WebApi.Helpers
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;
        private readonly IHttpContextAccessor _context;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider,IHttpContextAccessor context)
        {
            this.provider = provider;
           _context = context;
        }
            

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                         CreateInfoForApiVersion(description));
                        //new OpenApiInfo
                        //{
                        //    Title = $"MrCMS API {description.ApiVersion}",
                        //    Version = description.ApiVersion.ToString(),
                        //});
            }
        }
        

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var provider = _context.HttpContext.RequestServices.GetRequiredService<IStringResourceProvider>();
            var info = new OpenApiInfo()
            {
               Title = provider.GetValue("ApiTitle", "MrCMS API"),
                Version = description.ApiVersion.ToString(),
                Description = provider.GetValue("ApiDescription", "A MrCMS based application with Swagger, Swashbuckle, and API versioning."),
               Contact = new OpenApiContact { Name = provider.GetValue("ApiContactName", "Charles E."), Email = provider.GetValue("ApiContactEmail", "api@mrcms.co") },
               TermsOfService = new Uri(provider.GetValue("ApiTermsofServiceUrl", "https://opensource.org/licenses/MIT")),
                License = new OpenApiLicense() { Name = provider.GetValue("ApiLicenseName", "MIT"), Url = new Uri(provider.GetValue("ApiLicenseUrl", "https://opensource.org/licenses/MIT")) },
            };

            if (description.IsDeprecated)
            {
                info.Description += provider.GetValue("ApiDeprecatedInfo", " This API version has been deprecated.");
            }

            return info;
        }
    }
}