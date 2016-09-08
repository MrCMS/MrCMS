using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class WebpageWidgetAdminService : IWebpageWidgetAdminService
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IRepository<Widget> _widgetRepository;


        public WebpageWidgetAdminService(IRepository<Webpage> webpageRepository, IRepository<Widget> widgetRepository)
        {
            _webpageRepository = webpageRepository;
            _widgetRepository = widgetRepository;
        }

        public void Hide(Webpage webpage, int widgetId)
        {
            var widget = _widgetRepository.Get(widgetId);

            if (webpage == null || widget == null) return;

            if (webpage.ShownWidgets.Contains(widget))
                webpage.ShownWidgets.Remove(widget);
            else if (!webpage.HiddenWidgets.Contains(widget))
                webpage.HiddenWidgets.Add(widget);
            _webpageRepository.Update(webpage);
        }

        public void Show(Webpage webpage, int widgetId)
        {
            var widget = _widgetRepository.Get(widgetId);

            if (webpage == null || widget == null) return;

            if (webpage.HiddenWidgets.Contains(widget))
                webpage.HiddenWidgets.Remove(widget);
            else if (!webpage.ShownWidgets.Contains(widget))
                webpage.ShownWidgets.Add(widget);
            _webpageRepository.Update(webpage);
        }
    }
}