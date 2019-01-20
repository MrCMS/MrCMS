using System.Reflection;
using System.Web.Mvc;

namespace MrCMS.Website.Filters
{
    public class RequestActionAttribute : ActionMethodSelectorAttribute
    {
        private readonly string _actionName;

        public RequestActionAttribute(string actionName)
        {
            _actionName = actionName;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return controllerContext.HttpContext.Request["action"] == _actionName;
        }
    }
}