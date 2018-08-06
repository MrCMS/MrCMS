using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Helpers
{
    public static class PropertyRenderingExtensions
    {
        public static async Task<IHtmlContent> RenderCustomAdminProperties<T>(this IHtmlHelper htmlHelper, T entity, string suffix = null) where T : SystemEntity
        {
            if (entity == null)
                return HtmlString.Empty;
            var type = entity.GetType();

            return await RenderCustomAdminProperties(htmlHelper, suffix, type, entity);
        }

        private static async Task<IHtmlContent> RenderCustomAdminProperties(IHtmlHelper htmlHelper, string suffix, Type type, object entity)
        {
            //if (MrCMSApp.AppWebpages.ContainsKey(type))
            //    htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppWebpages[type];
            //if (MrCMSApp.AppWidgets.ContainsKey(type))
            //    htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppWidgets[type];

            // try and find an implementation of IImplementationPropertiesViewModel
            var modelType = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(IImplementationPropertiesViewModel<>).MakeGenericType(type)).FirstOrDefault();
            if (modelType == null)
                return HtmlString.Empty;

            var partialViewName = type.Name + (suffix ?? string.Empty);
            var viewEngine = htmlHelper.GetRequiredService<ICompositeViewEngine>();
            var actionContextAccessor = htmlHelper.GetRequiredService<IActionContextAccessor>();
            var mapper = htmlHelper.GetRequiredService<IMapper>();
            ViewEngineResult viewEngineResult = viewEngine.FindView(actionContextAccessor.ActionContext,
                partialViewName, false);
            if (viewEngineResult.View != null)
            {
                try
                {
                    var model = mapper.Map(entity, type, modelType);
                    return await htmlHelper.PartialAsync(partialViewName, model);
                }
                catch (Exception exception)
                {
                    // TODO: logging
                    //CurrentRequestData.ErrorSignal.Raise(exception);
                    return
                        new HtmlString(
                            "We could not find a custom admin view for this page. Either this page is bespoke or has no custom properties.");
                }
            }
            return HtmlString.Empty;
        }

        public static async Task<IHtmlContent> RenderUserOwnedObjectProperties(this IHtmlHelper<User> htmlHelper, Type entityType)
        {
            var user = htmlHelper.ViewData.Model;
            if (user == null)
                return HtmlString.Empty;
            // TODO: app lookup
            //if (MrCMSApp.AppEntities.ContainsKey(entityType))
            //    htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppEntities[entityType];

            var viewEngine = htmlHelper.GetRequiredService<ICompositeViewEngine>();
            var actionContextAccessor = htmlHelper.GetRequiredService<IActionContextAccessor>();
            ViewEngineResult viewEngineResult = viewEngine.FindView(actionContextAccessor.ActionContext,
                entityType.Name, false);
            if (viewEngineResult.View != null)
            {
                try
                {
                    return await htmlHelper.PartialAsync(entityType.Name, user);
                }
                catch (Exception exception)
                {
                    // TODO: logging
                    //CurrentRequestData.ErrorSignal.Raise(exception);
                }
            }
            return HtmlString.Empty;
        }

        public static async Task<IHtmlContent> RenderUserProfileProperties(this IHtmlHelper<User> htmlHelper, Type userProfileType)
        {
            var user = htmlHelper.ViewData.Model;
            if (user == null)
                return HtmlString.Empty;
            // TODO: app lookup
            //if (MrCMSApp.AppUserProfileDatas.ContainsKey(userProfileType))
            //    htmlHelper.ViewContext.RouteData.DataTokens["app"] = MrCMSApp.AppUserProfileDatas[userProfileType];

            var viewEngine = htmlHelper.GetRequiredService<ICompositeViewEngine>();
            var actionContextAccessor = htmlHelper.GetRequiredService<IActionContextAccessor>();
            ViewEngineResult viewEngineResult = viewEngine.FindView(actionContextAccessor.ActionContext, userProfileType.Name, false);
            if (viewEngineResult.View != null)
            {
                try
                {
                    return await htmlHelper.PartialAsync(userProfileType.Name, user);
                }
                catch (Exception exception)
                {
                    // TODO: logging
                    //CurrentRequestData.ErrorSignal.Raise(exception);
                }
            }
            return HtmlString.Empty;
        }
    }
}