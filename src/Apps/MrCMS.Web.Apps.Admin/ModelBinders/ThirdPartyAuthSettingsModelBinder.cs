using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class ThirdPartyAuthSettingsModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var serviceProvider = bindingContext.HttpContext.RequestServices;
            var binder = new SimpleTypeModelBinder(typeof(ThirdPartyAuthSettings), serviceProvider.GetRequiredService<ILoggerFactory>());
            bindingContext.Model = serviceProvider
                .GetRequiredService<ISystemConfigurationProvider>().GetSystemSettings<ThirdPartyAuthSettings>();
            return binder.BindModelAsync(bindingContext);
        }
    }
}