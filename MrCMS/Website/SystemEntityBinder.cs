using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using ISession = NHibernate.ISession;

namespace MrCMS.Website
{
    public class SystemEntityBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var method = bindingContext.HttpContext.Request.Method;

            if (!HttpMethods.IsGet(method) && !HttpMethods.IsHead(method)) return;

            var modelName = "id";
            // Try to fetch the value of the argument by name
            var valueProviderResult =
                bindingContext.ValueProvider.GetValue(modelName);

            var modelType = bindingContext.ModelType;
            if (valueProviderResult == ValueProviderResult.None) return;
            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value)) return;

            if (!int.TryParse(value, out var id))
            {
                // Non-integer arguments result in model state errors
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    "Id must be an integer.");
                return;
            }

            var entity = await bindingContext.HttpContext.RequestServices.GetRequiredService<ISession>()
                             .GetAsync(modelType, id) ??
                         GetDefault(modelType);
            bindingContext.Result = ModelBindingResult.Success(entity);
        }


        private object GetDefault(Type modelType)
        {
            if (modelType.IsAbstract)
                return null;

            return Activator.CreateInstance(modelType);
        }
    }
}