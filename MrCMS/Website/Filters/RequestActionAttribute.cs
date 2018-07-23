using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace MrCMS.Website.Filters
{
    public class RequestActionAttribute : ActionMethodSelectorAttribute
    {
        private readonly string _actionName;

        public RequestActionAttribute(string actionName)
        {
            _actionName = actionName;
        }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            return routeContext.HttpContext.GetRouteValue("action")?.ToString() == _actionName;
        }
    }
}