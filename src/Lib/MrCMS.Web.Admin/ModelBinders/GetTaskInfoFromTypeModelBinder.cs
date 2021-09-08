using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.ModelBinders
{
    public class GetTaskInfoFromTypeModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueFromContext = bindingContext.ValueProvider.GetValue("type").FirstValue;
            var serviceProvider = bindingContext.HttpContext.RequestServices;
            var taskAdminService = serviceProvider.GetRequiredService<ITaskAdminService>();
            bindingContext.Model = await taskAdminService.GetTaskUpdateData(valueFromContext);
            var modelBinder = new SimpleTypeModelBinder(typeof(TaskUpdateData),
                serviceProvider.GetRequiredService<ILoggerFactory>());
            await modelBinder.BindModelAsync(bindingContext);
        }
    }
}