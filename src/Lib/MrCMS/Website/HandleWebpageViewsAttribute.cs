using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website
{
    public class HandleWebpageViewsAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionExecutedContext = await next();

            var result = actionExecutedContext.Result as ViewResult;
            if (!(result?.Model is Webpage webpage)) return;
            await actionExecutedContext.HttpContext.RequestServices.GetRequiredService<IProcessWebpageViews>().Process(result, webpage);
        }
    }
}