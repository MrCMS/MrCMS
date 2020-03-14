using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities.Widget;
using MrCMS.Services.Caching;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;

namespace MrCMS.Services
{
    public class WidgetUIService : IWidgetUIService
    {
        private readonly IGetWidgetCachingInfo _getWidgetCachingInfo;
        private readonly IHtmlCacheService _htmlCacheService;
        private readonly IWidgetModelService _widgetModelService;
        private readonly IRepository<Widget> _repository;

        public WidgetUIService(IGetWidgetCachingInfo getWidgetCachingInfo, IHtmlCacheService htmlCacheService, IWidgetModelService widgetModelService, IRepository<Widget> repository)
        {
            _getWidgetCachingInfo = getWidgetCachingInfo;
            _htmlCacheService = htmlCacheService;
            _widgetModelService = widgetModelService;
            _repository = repository;
        }

        public Task<IHtmlContent> GetContent(IViewComponentHelper helper, int id,
            Func<IViewComponentHelper, Task<IHtmlContent>> func)
        {
            return _htmlCacheService.GetContent(helper, _getWidgetCachingInfo.Get(id), func);
        }

        public async Task<(Widget Widget, object Model)> GetModel(int id)
        {
            var widget = _repository.GetDataSync(id);
            return (widget, await _widgetModelService.GetModel(widget));
        }
    }
}