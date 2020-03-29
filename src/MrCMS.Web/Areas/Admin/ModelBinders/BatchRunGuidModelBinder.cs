using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Batching.Entities;
using MrCMS.Data;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class BatchRunGuidModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var id = Convert.ToString(bindingContext.ActionContext.RouteData.Values["id"]);
            if (Guid.TryParse(id, out var guid))
            {
                bindingContext.Result = ModelBindingResult.Success(await bindingContext.HttpContext.RequestServices
                    .GetRequiredService<IRepository<BatchRun>>().Query().FirstOrDefaultAsync(x => x.Guid == guid));
            }
        }
    }
}