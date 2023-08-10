using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.Resources;

namespace MrCMS.Globalization
{
    public class UserProfileRequestCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var service = httpContext.RequestServices.GetService<IGetUserCultureInfo>();
            if (service != null)
            {
                var culture = service.Get(httpContext.User.GetUserCulture());
                var cultureResult = new ProviderCultureResult(culture.Name);
                    
                return cultureResult;
            }

            var result = new ProviderCultureResult("en-GB");
            return result;
        }
    }
}