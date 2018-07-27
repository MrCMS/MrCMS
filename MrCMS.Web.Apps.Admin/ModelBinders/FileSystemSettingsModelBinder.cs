using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class FileSystemSettingsModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelBinder = new SimpleTypeModelBinder(typeof(FileSystemSettings));
            bindingContext.Model = bindingContext.HttpContext.RequestServices.GetRequiredService<FileSystemSettings>();
            return modelBinder.BindModelAsync(bindingContext);
        }
    }
}