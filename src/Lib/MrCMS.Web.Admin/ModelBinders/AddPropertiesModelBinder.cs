using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Web.Admin.ModelBinders
{
    public class AddPropertiesModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var typeName =
                bindingContext.ValueProvider.GetValue("type").FirstValue.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)
                    [0];
            var entityType = TypeHelper.MappedClasses.FirstOrDefault(type => type.FullName == typeName);

            if (entityType != null && entityType.HasDefaultConstructor())
            {
                var bindModel = Activator.CreateInstance(entityType) as Webpage;

                var parentId = bindingContext.ValueProvider.GetValue("parentId").FirstValue;
                int id;
                if (int.TryParse(parentId, out id) && bindModel != null)
                {
                    bindModel.Parent = bindingContext.HttpContext.RequestServices.GetRequiredService<ISession>().Get<Webpage>(id);
                }

                bindingContext.Result = ModelBindingResult.Success(bindModel);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
            return Task.CompletedTask;

        }
    }
}