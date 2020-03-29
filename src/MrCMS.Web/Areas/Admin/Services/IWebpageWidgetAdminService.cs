using System.Threading.Tasks;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IWebpageWidgetAdminService
    {
        Task Hide(int webpageId, int widgetId);
        Task Show(int webpageId, int widgetId);
    }
}