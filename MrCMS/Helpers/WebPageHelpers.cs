using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using FluentNHibernate.Automapping;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using System.Linq;
using MrCMS.IoC;
using MrCMS.Website;
using NHibernate;
using NHibernate.Proxy;
using Ninject;

namespace MrCMS.Helpers
{
    public static class WebPageHelpers
    {
        public static MvcHtmlString RenderZone(this HtmlHelper<Webpage> helper, string areaName)
        {
            var page = helper.ViewData.Model;

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
                        stringBuilder.Append(helper.Action("Show", "Widget", new { id = widget.Id, page }).ToHtmlString());
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
}