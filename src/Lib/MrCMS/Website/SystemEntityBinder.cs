using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MrCMS.Website
{
    public class SystemEntityBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var method = bindingContext.HttpContext.Request.Method;

            if (!HttpMethods.IsGet(method) && !HttpMethods.IsHead(method)) return;

            var entity = await bindingContext.GetEntityById(bindingContext.ModelType);
            if (entity == null)
                return;
            bindingContext.Result = ModelBindingResult.Success(entity);
        }
    }
}