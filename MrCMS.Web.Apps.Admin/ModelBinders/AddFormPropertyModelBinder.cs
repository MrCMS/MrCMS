using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class AddFormPropertyModelBinder : IModelBinder
    {

        private static Type GetTypeByName(ModelBindingContext modelBindingContext)
        {
            return
                TypeHelper.GetAllConcreteTypesAssignableFrom<FormProperty>()
                          .FirstOrDefault(type => type.Name == modelBindingContext.ValueProvider.GetValue("type").FirstValue);
        }

        //public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        //{
        //}

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var type = GetTypeByName(bindingContext);
            if(type == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var metadataProvider = bindingContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();
            bindingContext.ModelMetadata = metadataProvider.GetMetadataForType(type);
            var modelBinder = new SimpleTypeModelBinder(type);

            await modelBinder.BindModelAsync(bindingContext);

            var formProperty = bindingContext.Result.Model as FormProperty;
            formProperty?.Webpage?.FormProperties.Add(formProperty);
            bindingContext.Result = ModelBindingResult.Success(formProperty);
        }
    }
}