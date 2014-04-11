using System.Web.Mvc;
using MrCMS.Tasks;
using Ninject;

namespace MrCMS.Website.Filters
{
    public class ForceImmediateLuceneUpdateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            CurrentRequestData.OnEndRequest.Add(kernel => kernel.Get<ITaskRunner>().ExecuteLuceneTasks());
        }
    }
}