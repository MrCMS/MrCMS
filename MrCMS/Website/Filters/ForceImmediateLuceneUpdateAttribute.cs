using System.Web.Mvc;
using Ninject;

namespace MrCMS.Website.Filters
{
    public class ForceImmediateLuceneUpdateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            CurrentRequestData.OnEndRequest.Add(new ExecuteLuceneTasks());
        }
    }
}