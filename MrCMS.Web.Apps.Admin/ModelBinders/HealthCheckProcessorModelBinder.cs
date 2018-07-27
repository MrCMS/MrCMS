using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class HealthCheckProcessorModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string typeName = bindingContext.ValueProvider.GetValue("typeName").FirstValue;
            var typeByName = TypeHelper.GetTypeByName(typeName);
            bindingContext.Result = ModelBindingResult.Success(typeName == null
                ? null
                : bindingContext.HttpContext.RequestServices.GetService(typeByName));

            return Task.CompletedTask;
        }
    }
}