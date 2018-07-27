using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class ThirdPartyAuthSettingsModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var binder = new SimpleTypeModelBinder(typeof(ThirdPartyAuthSettings));
            bindingContext.Model = bindingContext.HttpContext.RequestServices
                .GetRequiredService<ISystemConfigurationProvider>().GetSystemSettings<ThirdPartyAuthSettings>();
            return binder.BindModelAsync(bindingContext);
        }
    }
}