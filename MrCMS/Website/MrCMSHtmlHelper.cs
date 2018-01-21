using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace MrCMS.Website
{
    public class MrCMSHtmlHelper : IHtmlHelper
    {
        private readonly HtmlHelper _helper;

        public MrCMSHtmlHelper(HtmlHelper helper)
        {
            _helper = helper;
        }

        public void RenderAction(string actionName)
            => _helper.RenderAction(actionName);

        public void RenderAction(string actionName, string controllerName)
            => _helper.RenderAction(actionName, controllerName);

        public void RenderAction(string actionName, object routeValues)
            => _helper.RenderAction(actionName, routeValues);

        public void RenderAction(string actionName, RouteValueDictionary routeValues)
            => _helper.RenderAction(actionName, routeValues);

        public void RenderAction(string actionName, string controllerName, object routeValues)
            => _helper.RenderAction(actionName, controllerName, routeValues);

        public void RenderAction(string actionName, string controllerName, RouteValueDictionary routeValues)
            => _helper.RenderAction(actionName, controllerName, routeValues);


        public MvcHtmlString Action(string actionName)
            => _helper.Action(actionName);

        public MvcHtmlString Action(string actionName, string controllerName)
            => _helper.Action(actionName, controllerName);

        public MvcHtmlString Action(string actionName, object routeValues)
            => _helper.Action(actionName, routeValues);

        public MvcHtmlString Action(string actionName, RouteValueDictionary routeValues)
            => _helper.Action(actionName, routeValues);

        public MvcHtmlString Action(string actionName, string controllerName, object routeValues)
            => _helper.Action(actionName, controllerName, routeValues);

        public MvcHtmlString Action(string actionName, string controllerName, RouteValueDictionary routeValues)
            => _helper.Action(actionName, controllerName, routeValues);


        public void RenderPartial(string viewName)
            => _helper.RenderPartial(viewName);
        public ViewContext ViewContext => _helper.ViewContext;
    }
}