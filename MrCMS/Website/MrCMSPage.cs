using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.ACL.Rules;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Settings;
using MrCMS.Helpers;
using StackExchange.Profiling;

namespace MrCMS.Website
{
    public abstract class MrCMSPage<TModel> : WebViewPage<TModel>
    {
        public const string PageLayoutAreas = "page.current.layout.areas";
        public const string PageCurrentLayout = "page.current.layout";
        private IConfigurationProvider _configurationProvider;
        private IStringResourceProvider _stringResourceProvider;
        private IGetCurrentLayout _getCurrentLayout;

        public T SiteSettings<T>() where T : SiteSettingsBase, new()
        {
            return _configurationProvider.GetSiteSettings<T>();
        }

        public string Resource(string key, string defaultValue = null)
        {
            return _stringResourceProvider.GetValue(key, defaultValue);
        }

        public override void InitHelpers()
        {
            base.InitHelpers();

            if (CurrentRequestData.DatabaseIsInstalled)
            {
                _configurationProvider = MrCMSApplication.Get<IConfigurationProvider>();
                _stringResourceProvider = MrCMSApplication.Get<IStringResourceProvider>();
                GetCurrentLayout = MrCMSApplication.Get<IGetCurrentLayout>();
            }
        }

        public MvcHtmlString Editable<T>(T model, Expression<Func<T, string>> method, bool isHtml = false) where T : SystemEntity
        {
            if (model == null)
                return MvcHtmlString.Empty;

            var propertyInfo = PropertyFinder.GetProperty(method);

            var value = Html.ParseShortcodes(method.Compile().Invoke(model)).ToHtmlString();

            var typeName = model.GetType().Name;

            if (EditingEnabled && propertyInfo != null)
            {
                var tagBuilder = new TagBuilder(isHtml ? "div" : "span");
                tagBuilder.AddCssClass("editable");
                tagBuilder.Attributes["data-id"] = model.Id.ToString();
                tagBuilder.Attributes["data-property"] = propertyInfo.Name;
                tagBuilder.Attributes["data-type"] = typeName;
                tagBuilder.Attributes["data-is-html"] = isHtml ? "true" : "false";
                tagBuilder.InnerHtml = value;

                return MvcHtmlString.Create(tagBuilder.ToString());
            }
            return MvcHtmlString.Create(value);
        }

        private bool EditingEnabled
        {
            get
            {
                return CurrentRequestData.CurrentUser != null &&
                       CurrentRequestData.CurrentUser.CanAccess<AdminBarACL>("Show") &&
                       _configurationProvider.GetSiteSettings<SiteSettings>().EnableInlineEditing;
            }
        }

        public IGetCurrentLayout GetCurrentLayout
        {
            get { return _getCurrentLayout; }
            set { _getCurrentLayout = value; }
        }

        public void RenderZone(string areaName, Webpage page = null, bool allowFrontEndEditing = true)
        {
            page = page ?? CurrentRequestData.CurrentPage;

            var currentLayout = Context.Items[PageCurrentLayout] as Layout;
            if (currentLayout == null)
            {
                currentLayout = GetCurrentLayout.Get(page);
                Context.Items[PageCurrentLayout] = currentLayout;
            }
            var layoutAreas = Context.Items[PageLayoutAreas] as IEnumerable<LayoutArea>;
            if (layoutAreas == null)
            {
                layoutAreas = currentLayout.GetLayoutAreas();
                Context.Items[PageLayoutAreas] = layoutAreas;
            }
            if (page != null && currentLayout != null)
            {
                var allowEdit = EditingEnabled && allowFrontEndEditing;

                var layoutArea = layoutAreas.FirstOrDefault(area => area.AreaName == areaName);

                if (layoutArea == null) return;

                bool customSort = layoutArea.PageWidgetSorts.Any(sort => sort.Webpage == page);

                if (allowEdit)
                    ViewContext.Writer.Write(
                        "<div data-layout-area-id=\"{0}\" data-layout-area-name=\"{1}\" " +
                        "data-layout-area-hascustomsort=\"{2}\" class=\"layout-area\"> ",
                        layoutArea.Id, layoutArea.AreaName, customSort);

                foreach (var widget in layoutArea.GetWidgets(page))
                {
                    if (allowEdit)
                        ViewContext.Writer.Write(
                            "<div data-widget-id=\"{0}\" data-widget-name=\"{1}\" class=\"widget\"> ", widget.Id,
                            widget.Name ?? widget.GetType().Name);

                    try
                    {

                        Html.RenderAction("Show", "Widget", new { widget });
                    }
                    catch (Exception ex)
                    {
                        CurrentRequestData.ErrorSignal.Raise(ex);
                    }

                    if (allowEdit)
                        ViewContext.Writer.Write("</div>");
                }

                if (allowEdit)
                    ViewContext.Writer.Write("</div>");
            }
        }

        public MvcHtmlString RenderImage(string imageUrl, Size size = default(Size), string alt = null, string title = null, object attributes = null)
        {
            using (MiniProfiler.Current.Step(string.Format("Render image - {0}, size {1}", imageUrl, size)))
                return Html.RenderImage(imageUrl, size, alt, title, attributes);
        }
    }

    public abstract class MrCMSPage : MrCMSPage<dynamic>
    {
    }
}