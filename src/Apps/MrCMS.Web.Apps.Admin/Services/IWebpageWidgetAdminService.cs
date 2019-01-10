using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IWebpageWidgetAdminService
    {
        void Hide(int webpageId, int widgetId);
        void Show(int webpageId, int widgetId);
    }
}