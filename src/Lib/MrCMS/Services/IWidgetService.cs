using MrCMS.Entities.Widget;

namespace MrCMS.Services
{
    public interface IWidgetService
    {
        T GetWidget<T>(int id) where T : Widget;
        void SaveWidget(Widget widget);
        void DeleteWidget(Widget widget);
        Widget AddWidget(Widget widget);
    }
}