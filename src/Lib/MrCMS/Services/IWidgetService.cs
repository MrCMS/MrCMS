using System.Threading.Tasks;
using MrCMS.Entities.Widget;

namespace MrCMS.Services
{
    public interface IWidgetService
    {
        Task<T> GetWidget<T>(int id) where T : Widget;
        Task SaveWidget(Widget widget);
        Task DeleteWidget(Widget widget);
        Task<Widget> AddWidget(Widget widget);
    }
}