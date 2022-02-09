using AutoMapper;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Apps;
using MrCMS.Web.Admin.Infrastructure.ModelBinding;
using MrCMS.Website;

namespace MrCMS.Web.Admin.Infrastructure.Helpers
{
    public static class PropertyRenderingExtensions
    {
        private readonly static HashSet<Type> UpdateModelTypes =
            TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(IUpdatePropertiesViewModel<>));

        public static async Task<IHtmlContent> RenderUpdateCustomAdminProperties<T>(this IHtmlHelper htmlHelper,
            T entity) where T : SystemEntity
        {
            if (entity == null)
            {
                return HtmlString.Empty;
            }

            var type = entity.Unproxy().GetType();

            var modelType = UpdateModelTypes.FirstOrDefault(x =>
                typeof(IUpdatePropertiesViewModel<>).MakeGenericType(type).IsAssignableFrom(x));
            if (modelType == null)
            {
                return HtmlString.Empty;
            }

            var mapper = htmlHelper.GetRequiredService<IMapper>();
            var model = mapper.Map(entity, type, modelType);

            return await RenderCustomAdminProperties(htmlHelper, String.Empty, type, model);
        }

        public static Task<IHtmlContent> RenderCustomAddAdminProperties(this IHtmlHelper htmlHelper, string typeName)
        {
            return RenderCustomAddAdminProperties(htmlHelper, TypeHelper.GetTypeByName(typeName));
        }

        public static Task<IHtmlContent> RenderCustomAddAdminProperties(this IHtmlHelper htmlHelper, Type type)
        {
            return RenderCustomAddAdminProperties(htmlHelper, type, Activator.CreateInstance(type));
        }

        public static Task<IHtmlContent> RenderCustomAddAdminProperties(this IHtmlHelper htmlHelper, Type type,
            object model)
        {
            return RenderCustomAdminProperties(htmlHelper, "Add", type, model);
        }

        private static async Task<IHtmlContent> RenderCustomAdminProperties(IHtmlHelper htmlHelper, string suffix,
            Type type, object model)
        {
            // try and find an implementation of IUpdatePropertiesViewModel

            var partialViewName = type.Name + (suffix ?? string.Empty);
            var viewEngine = htmlHelper.GetRequiredService<ICompositeViewEngine>();
            var actionContextAccessor = htmlHelper.GetRequiredService<IActionContextAccessor>();
            var logger = htmlHelper.GetRequiredService<ILogger<CompositeViewEngine>>();
            ViewEngineResult viewEngineResult = viewEngine.FindView(actionContextAccessor.ActionContext,
                partialViewName, false);
            if (viewEngineResult.View != null)
            {
                try
                {
                    return await htmlHelper.PartialAsync(partialViewName, model);
                }
                catch (Exception exception)
                {
                    logger.Log(LogLevel.Error, exception, exception.Message);
                    return
                        new HtmlString(
                            "We could not find a custom admin view for this page. Either this page is bespoke or has no custom properties.");
                }
            }

            return HtmlString.Empty;
        }

        public static async Task<IHtmlContent> RenderUserOwnedObjectProperties(this IHtmlHelper<User> htmlHelper,
            Type entityType)
        {
            var user = htmlHelper.ViewData.Model;
            if (user == null)
            {
                return HtmlString.Empty;
            }

            SetApp(htmlHelper, entityType);

            var viewEngine = htmlHelper.GetRequiredService<ICompositeViewEngine>();
            var actionContextAccessor = htmlHelper.GetRequiredService<IActionContextAccessor>();
            var logger = htmlHelper.GetRequiredService<ILogger<CompositeViewEngine>>();
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
                    logger.Log(LogLevel.Error, exception, exception.Message);
                }
            }

            return HtmlString.Empty;
        }

        private static void SetApp(IHtmlHelper<User> htmlHelper, Type type)
        {
            var appContext = htmlHelper.GetRequiredService<MrCMSAppContext>();
            if (appContext.Types.ContainsKey(type))
                htmlHelper.ViewContext.RouteData.DataTokens[AppViewLocationExpander.Key] =
                    appContext.Types[type].Name;
        }

        public static async Task<IHtmlContent> RenderUserProfileProperties(this IHtmlHelper<User> htmlHelper,
            Type userProfileType)
        {
            var user = htmlHelper.ViewData.Model;
            if (user == null)
            {
                return HtmlString.Empty;
            }

            SetApp(htmlHelper, userProfileType);

            var viewEngine = htmlHelper.GetRequiredService<ICompositeViewEngine>();
            var actionContextAccessor = htmlHelper.GetRequiredService<IActionContextAccessor>();
            var logger = htmlHelper.GetRequiredService<ILogger<CompositeViewEngine>>();
            ViewEngineResult viewEngineResult =
                viewEngine.FindView(actionContextAccessor.ActionContext, userProfileType.Name, false);
            if (viewEngineResult.View != null)
            {
                try
                {
                    return await htmlHelper.PartialAsync(userProfileType.Name, user);
                }
                catch (Exception exception)
                {
                    logger.Log(LogLevel.Error, exception, exception.Message);
                }
            }

            return HtmlString.Empty;
        }
    }
}