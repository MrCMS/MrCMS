using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class GetTaskInfoFromTypeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueFromContext = bindingContext.ValueProvider.GetValue("type").FirstValue;
            var taskAdminService = bindingContext.HttpContext.RequestServices.GetRequiredService<ITaskAdminService>();
            bindingContext.Model = taskAdminService.GetTaskUpdateData(valueFromContext);
            var modelBinder = new SimpleTypeModelBinder(typeof(TaskUpdateData));
            return modelBinder.BindModelAsync(bindingContext);
            throw new NotImplementedException();
        }
    }
}