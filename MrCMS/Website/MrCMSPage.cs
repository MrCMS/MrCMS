using System;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.ACL.Rules;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Settings;
using MrCMS.Helpers;

namespace MrCMS.Website
{
    public abstract class MrCMSPage<TModel> : WebViewPage<TModel>
    {
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

            var typeName = "";
            if (model is Webpage) //get base document type as using generic interfaces cause issues using Editable I.E DocumentContainer
                typeName = (model as Webpage).DocumentType;
            else
                typeName = model.GetType().Name;

            if (EditingEnabled && propertyInfo != null)
            {
                var tagBuilder = new TagBuilder("div");
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

            var currentLayout = GetCurrentLayout.Get(page);
            if (page != null && currentLayout != null)
            {
                var allowEdit = EditingEnabled && allowFrontEndEditing;

                var layoutArea = currentLayout.GetLayoutAreas().FirstOrDefault(area => area.AreaName == areaName);

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
            return Html.RenderImage(imageUrl, size, alt, title, attributes);
        }
    }

    public abstract class MrCMSPage : MrCMSPage<dynamic>
    {
    }
}