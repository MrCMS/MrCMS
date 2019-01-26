using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website
{
    public class HandleWebpageViewsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var result = filterContext.Result as ViewResult;
            if (result == null) return;
            var webpage = result.Model as Webpage;
            if (webpage == null) return;
            filterContext.HttpContext.RequestServices.GetRequiredService<IProcessWebpageViews>().Process(result, webpage);
        }
    }
}