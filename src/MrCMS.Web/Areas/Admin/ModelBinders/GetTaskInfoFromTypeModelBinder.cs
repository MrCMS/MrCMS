using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class GetTaskInfoFromTypeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueFromContext = bindingContext.ValueProvider.GetValue("type").FirstValue;
            var serviceProvider = bindingContext.HttpContext.RequestServices;
            var taskAdminService = serviceProvider.GetRequiredService<ITaskAdminService>();
            bindingContext.Model = taskAdminService.GetTaskUpdateData(valueFromContext);
            var modelBinder = new SimpleTypeModelBinder(typeof(TaskUpdateData), serviceProvider.GetRequiredService<ILoggerFactory>());
            return modelBinder.BindModelAsync(bindingContext);
            throw new NotImplementedException();
        }
    }
}