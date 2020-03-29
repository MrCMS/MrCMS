using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class GetUrlGeneratorOptionsTypeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var typeName = bindingContext.ValueProvider.GetValue("type").FirstValue;
            bindingContext.Result = ModelBindingResult.Success(TypeHelper.GetTypeByName(typeName));
            return Task.CompletedTask;
        }
    }
}