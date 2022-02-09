using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;

namespace MrCMS.Web.Admin.ModelBinders
{
    public class AddWidgetPropertiesModelBinder : IModelBinder
    {

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var typeName =
                bindingContext.ValueProvider.GetValue("type").FirstValue.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)
                    [0];
            var entityType = TypeHelper.MappedClasses.FirstOrDefault(type => type.FullName == typeName);

            if (entityType != null && entityType.HasDefaultConstructor())
            {
                bindingContext.Result = ModelBindingResult.Success(Activator.CreateInstance(entityType) as Widget);
            }

            return Task.CompletedTask;
        }
    }
}