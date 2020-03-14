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

        public async Task<IHtmlContent> GetContent(IViewComponentHelper helper, int id,
            Func<IViewComponentHelper, Task<IHtmlContent>> func)
        {
            return await _htmlCacheService.GetContent(helper, _getWidgetCachingInfo.Get(id), func);
        }

        public Task<object> GetModel(Widget widget)
        {
            return _widgetModelService.GetModel(widget);
        }

        public Task<Widget> GetWidget(int id)
        {
            return _repository.GetData(id);
        }
    }
}