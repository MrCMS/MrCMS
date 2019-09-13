using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using MrCMS.Web.Apps.WebApi.Api;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MrCMS.Web.Apps.WebApi.Filters
{
    public class WebApiOperationFilter : IOperationFilter
    {
       
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<MrCMSWebApiAttribute>();

            if (authAttributes.Any())
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        }
    }
}
