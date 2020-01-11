using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IWebpageWidgetAdminService
    {
        Task Hide(int webpageId, int widgetId);
        Task Show(int webpageId, int widgetId);
    }
}