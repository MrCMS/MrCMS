using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Batching.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class BatchRunGuidModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var id = Convert.ToString(bindingContext.ActionContext.RouteData.Values["id"]);
            if (Guid.TryParse(id, out var guid))
            {
                bindingContext.Result = ModelBindingResult.Success(bindingContext.HttpContext.RequestServices
                    .GetRequiredService<ISession>().Query<BatchRun>().FirstOrDefault(x => x.Guid == guid));
            }

            return Task.CompletedTask;
        }
    }
}