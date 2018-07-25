using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities.Widget;
using NHibernate;

namespace MrCMS.Services
{
    public class WidgetUIService : IWidgetUIService
    {
        //private readonly IGetWidgetCachingInfo _getWidgetCachingInfo;
        //private readonly IHtmlCacheService _htmlCacheService;
        private readonly IWidgetModelService _widgetModelService;
        private readonly ISession _session;

        // TODO: complete and add caching
        public WidgetUIService(/*IGetWidgetCachingInfo getWidgetCachingInfo, IHtmlCacheService htmlCacheService,*/ IWidgetModelService widgetModelService, ISession session)
        {
            //_getWidgetCachingInfo = getWidgetCachingInfo;
            //_htmlCacheService = htmlCacheService;
            _widgetModelService = widgetModelService;
            _session = session;
        }

        public ActionResult GetContent(Controller controller, Widget widget, Func<IHtmlHelper, IHtmlContent> func)
        {
            //return _htmlCacheService.GetContent(controller, _getWidgetCachingInfo.Get(widget), func);
            throw new NotImplementedException("Not yet implemented");
        }

        public (Widget Widget, object Model) GetModel(int id)
        {
            var widget = _session.Get<Widget>(id);
            return (widget, GetModel(widget));
        }

        public object GetModel(Widget widget)
        {
            return _widgetModelService.GetModel(widget);
        }

        public void SetAppDataToken(RouteData routeData, Widget widget)
        {
            throw new NotImplementedException("Not yet implemented");
            //if (MrCMSApp.AppWidgets.ContainsKey(widget.Unproxy().GetType()))
            //    routeData.DataTokens["app"] = MrCMSApp.AppWidgets[widget.Unproxy().GetType()];
        }
    }
}