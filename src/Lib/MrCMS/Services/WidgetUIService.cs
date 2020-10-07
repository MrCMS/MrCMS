using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities.Widget;
using MrCMS.Services.Caching;
using NHibernate;
using System;
using System.Threading.Tasks;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class WidgetUIService : IWidgetUIService
    {
        private readonly IGetWidgetCachingInfo _getWidgetCachingInfo;
        private readonly IHtmlCacheService _htmlCacheService;
        private readonly IWidgetModelService _widgetModelService;
        private readonly ISession _session;

        public WidgetUIService(IGetWidgetCachingInfo getWidgetCachingInfo, IHtmlCacheService htmlCacheService, IWidgetModelService widgetModelService, ISession session)
        {
            _getWidgetCachingInfo = getWidgetCachingInfo;
            _htmlCacheService = htmlCacheService;
            _widgetModelService = widgetModelService;
            _session = session;
        }

        public IHtmlContent GetContent(IViewComponentHelper helper, int id, Func<IViewComponentHelper, Task<IHtmlContent>> func)
        {
            var htmlContent = _htmlCacheService.GetContent(helper, _getWidgetCachingInfo.Get(id), func);
            return htmlContent;
        }

        public (Widget Widget, object Model) GetModel(int id)
        {
            var widget = _session.Get<Widget>(id).Unproxy();
            return (widget, _widgetModelService.GetModel(widget));
        }
    }
}