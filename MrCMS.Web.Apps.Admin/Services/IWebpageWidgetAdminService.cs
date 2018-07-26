using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IWebpageWidgetAdminService
    {
        void Hide(Webpage webpage, int widgetId);
        void Show(Webpage webpage, int widgetId);
    }
}