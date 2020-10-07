using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Settings;

namespace MrCMS.Globalization
{
    public class UserProfileRequestCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var service = httpContext.RequestServices.GetService<IGetUserCultureInfo>();
            var userService = httpContext.RequestServices.GetService<IGetCurrentUser>();
            if (service != null)
            {
                var culture = service.Get(userService?.Get());
                var cultureResult = new ProviderCultureResult(culture.Name);
                return Task.FromResult(cultureResult);
            }

            var result = new ProviderCultureResult("en-GB");
            return Task.FromResult(result);
        }
    }
}