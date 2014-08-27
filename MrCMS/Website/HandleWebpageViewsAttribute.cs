using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

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
            MrCMSApplication.Get<IProcessWebpageViews>().Process(result, webpage);
        }
    }
}