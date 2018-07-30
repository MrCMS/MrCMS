using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class FileSystemSettingsModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var serviceProvider = bindingContext.HttpContext.RequestServices;
            var modelBinder = new SimpleTypeModelBinder(typeof(FileSystemSettings),serviceProvider.GetRequiredService<ILoggerFactory>());
            bindingContext.Model = serviceProvider.GetRequiredService<FileSystemSettings>();
            return modelBinder.BindModelAsync(bindingContext);
        }
    }
}