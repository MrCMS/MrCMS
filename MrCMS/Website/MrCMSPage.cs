using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public abstract class MrCMSPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private IConfigurationProvider _configurationProvider;
        private ISiteService _siteService;

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
            }
        }

        public MvcHtmlString RenderZone( string areaName)
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
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
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
    }

    public abstract class MrCMSPage : MrCMSPage<dynamic>
    {
    }
}