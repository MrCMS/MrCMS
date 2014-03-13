using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Apps;
using MrCMS.Entities.People;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class PropertyRenderingExtensions
    {
        public static MvcHtmlString RenderCustomAdminProperties<T>(this HtmlHelper<T> htmlHelper, string suffix = null) where T : class
        {
            T model = htmlHelper.ViewData.Model;
            if (model == null)
                return MvcHtmlString.Empty;
            var type = model.GetType();

            return RenderCustomAdminProperties(htmlHelper, suffix, type, model);
        }

        private static MvcHtmlString RenderCustomAdminProperties(HtmlHelper htmlHelper, string suffix, Type type, object model)
        {
            if (MrCMSApp.AppWebpages.ContainsKey(type))
                htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppWebpages[type];
            if (MrCMSApp.AppWidgets.ContainsKey(type))
                htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppWidgets[type];

            var partialViewName = type.Name + (suffix ?? string.Empty);
            ViewEngineResult viewEngineResult =
                ViewEngines.Engines.FindView(
                    new ControllerContext(htmlHelper.ViewContext.RequestContext, htmlHelper.ViewContext.Controller),
                    partialViewName, "");
            if (viewEngineResult.View != null)
            {
                try
                {
                    return htmlHelper.Partial(partialViewName, model);
                }
                catch (Exception exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                    return
                        MvcHtmlString.Create(
                            "We could not find a custom admin view for this page. Either this page is bespoke or has no custom properties.");
                }
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString RenderUserOwnedObjectProperties(this HtmlHelper<User> htmlHelper, Type entityType)
        {
            var user = htmlHelper.ViewData.Model;
            if (user == null)
                return MvcHtmlString.Empty;
            if (MrCMSApp.AppEntities.ContainsKey(entityType))
                htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppEntities[entityType];

            ViewEngineResult viewEngineResult =
                ViewEngines.Engines.FindView(new ControllerContext(htmlHelper.ViewContext.RequestContext, htmlHelper.ViewContext.Controller), entityType.Name, "");
            if (viewEngineResult.View != null)
            {
                try
                {
                    return htmlHelper.Partial(entityType.Name, user);
                }
                catch (Exception exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                }
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString RenderUserProfileProperties(this HtmlHelper<User> htmlHelper, Type userProfileType)
        {
            var user = htmlHelper.ViewData.Model;
            if (user == null)
                return MvcHtmlString.Empty;
            if (MrCMSApp.AppUserProfileDatas.ContainsKey(userProfileType))
                htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppUserProfileDatas[userProfileType];

            ViewEngineResult viewEngineResult =
                ViewEngines.Engines.FindView(new ControllerContext(htmlHelper.ViewContext.RequestContext, htmlHelper.ViewContext.Controller), userProfileType.Name, "");
            if (viewEngineResult.View != null)
            {
                try
                {
                    return htmlHelper.Partial(userProfileType.Name, user);
                }
                catch (Exception exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                }
            }
            return MvcHtmlString.Empty;
        }
    }
    public static class TypeExtensions
    {
        public static bool HasDefaultConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}