using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class WebpageWidgetAdminService : IWebpageWidgetAdminService
    {
        private readonly IDocumentService _documentService;
        private readonly ISession _session;

        public WebpageWidgetAdminService(IDocumentService documentService, ISession session)
        {
            _documentService = documentService;
            _session = session;
        }

        public void Hide(Webpage webpage, int widgetId)
        {
            var widget = _session.Get<Widget>(widgetId);

            if (webpage == null || widget == null) return;

            if (webpage.ShownWidgets.Contains(widget))
                webpage.ShownWidgets.Remove(widget);
            else if (!webpage.HiddenWidgets.Contains(widget))
                webpage.HiddenWidgets.Add(widget);
            _documentService.SaveDocument(webpage);
        }

        public void Show(Webpage webpage, int widgetId)
        {
            var widget = _session.Get<Widget>(widgetId);

            if (webpage == null || widget == null) return;

            if (webpage.HiddenWidgets.Contains(widget))
                webpage.HiddenWidgets.Remove(widget);
            else if (!webpage.ShownWidgets.Contains(widget))
                webpage.ShownWidgets.Add(widget);
            _documentService.SaveDocument(webpage);

        }
    }
}