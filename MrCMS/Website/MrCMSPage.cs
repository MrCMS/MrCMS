using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Helpers;

namespace MrCMS.Website
{
    public abstract class MrCMSPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private IConfigurationProvider _configurationProvider;
        private ISiteService _siteService;
        private IImageProcessor _imageProcessor;

        public T SiteSettings<T>() where T : SiteSettingsBase, new()
        {
            return _configurationProvider.GetSettings<T>(_siteService.GetCurrentSite());
        }

        public T GlobalSettings<T>() where T : GlobalSettingsBase, new()
        {
            return _configurationProvider.GetSettings<T>();
        }

        public override void InitHelpers()
        {
            base.InitHelpers();

            if (MrCMSApplication.DatabaseIsInstalled)
            {
                _siteService = MrCMSApplication.Get<ISiteService>();
                _configurationProvider = MrCMSApplication.Get<IConfigurationProvider>();
                _imageProcessor = MrCMSApplication.Get<IImageProcessor>();
            }
        }

        public MvcHtmlString Editable<T>(T model, Expression<Func<T, string>> method) where T : BaseEntity
        {
            if (model == null)
                return MvcHtmlString.Empty;

            var value = method.Compile().Invoke(model);
            var typeName = typeof(T).Name;
            var propertyInfo = PropertyFinder.GetProperty(method);

            if (MrCMSApplication.CurrentUserIsAdmin && propertyInfo != null)
            {
                var tagBuilder = new TagBuilder("div");
                tagBuilder.AddCssClass("editable");
                tagBuilder.Attributes["data-id"] = model.Id.ToString();
                tagBuilder.Attributes["data-property"] = propertyInfo.Name;
                tagBuilder.Attributes["data-type"] = typeName;
                tagBuilder.Attributes["contenteditable"] = "true";
                tagBuilder.InnerHtml = value;

                return MvcHtmlString.Create(tagBuilder.ToString());
            }
            else
            {
                return MvcHtmlString.Create(value);
            }
        }

        public class PropertyFinder
        {
            public static PropertyInfo GetProperty(Expression expression)
            {
                if (expression is MemberExpression && (expression as MemberExpression).Member is PropertyInfo)
                {
                    return ((expression as MemberExpression).Member as PropertyInfo);
                }
                return null;
            }
        }

        public MvcHtmlString RenderZone(string areaName)
        {
            var page = Model as Webpage;

            if (page != null && page.CurrentLayout != null)
            {
                var layout = page.CurrentLayout;

                var layoutArea = layout.GetLayoutAreas().FirstOrDefault(area => area.AreaName == areaName);

                if (layoutArea == null) return MvcHtmlString.Empty;

                var stringBuilder = new StringBuilder();
                if (MrCMSApplication.CurrentUserIsAdmin)
                    stringBuilder.AppendFormat("<div data-layout-area-id=\"{0}\" class=\"layout-area\"> ", layoutArea.Id);

                foreach (var widget in layoutArea.GetWidgets(page))
                {
                    if (MrCMSApplication.CurrentUserIsAdmin)
                        stringBuilder.AppendFormat("<div data-widget-id=\"{0}\" class=\"widget\"> ", widget.Id);

                    try
                    {
                        stringBuilder.Append(Html.Action("Show", "Widget", new { id = widget.Id, page }).ToHtmlString());
                    }
                    catch (Exception ex)
                    {
                        MrCMSApplication.ErrorSignal.Raise(ex);
                    }

                    if (MrCMSApplication.CurrentUserIsAdmin)
                        stringBuilder.Append("</div>");
                }

                if (MrCMSApplication.CurrentUserIsAdmin)
                    stringBuilder.Append("</div>");

                return MvcHtmlString.Create(stringBuilder.ToString());
            }
            return MvcHtmlString.Empty;
        }

        public MvcHtmlString RenderImage(string imageUrl, string alt = null, string title = null, object attributes = null)
        {
            return Html.RenderImage(imageUrl, alt, title, attributes);
        }

        public MvcHtmlString RenderImage(string imageUrl, Size size, string alt = null, string title = null, object attributes = null)
        {
            return Html.RenderImage(imageUrl, size, alt, title, attributes);
        }
    }

    public abstract class MrCMSPage : MrCMSPage<dynamic>
    {
    }
}