using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website
{
    public class ProcessWebpageViews : IProcessWebpageViews
    {
        public const string WidgetData = "widget-data";
        public const string LayoutFile = "layout-file";
        public const string CurrentPage = "current-page";

        private readonly IGetCurrentLayout _getCurrentLayout;
        private readonly IGetWidgetDisplayInfo _getWidgetDisplayInfo;

        public ProcessWebpageViews(IGetCurrentLayout getCurrentLayout, IGetWidgetDisplayInfo getWidgetDisplayInfo)
        {
            _getCurrentLayout = getCurrentLayout;
            _getWidgetDisplayInfo = getWidgetDisplayInfo;
        }

        public void Process(ViewResult result, Webpage webpage)
        {
            result.ViewData[CurrentPage] = webpage;

            if (string.IsNullOrWhiteSpace(result.ViewName))
            {
                if (webpage.PageTemplate != null && !string.IsNullOrWhiteSpace(webpage.PageTemplate.PageTemplateName))
                    result.ViewName = webpage.PageTemplate.PageTemplateName;
                else
                    result.ViewName = webpage.GetType().Name;
            }

            if (string.IsNullOrWhiteSpace(result.ViewData[LayoutFile]?.ToString()))
            {
                Layout layout = _getCurrentLayout.Get(webpage);
                if (layout != null)
                {
                    result.ViewData[LayoutFile] = layout.GetLayoutName();
                    result.ViewData[WidgetData] = _getWidgetDisplayInfo.GetWidgets(layout);
                }
            }
        }
    }
}