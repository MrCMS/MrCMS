using MrCMS.Entities.Widget;

namespace MrCMS.Services
{
    public interface IWidgetService
    {
        T GetWidget<T>(int id) where T : Widget;
        void SaveWidget(Widget widget);
        /// <summary>
        /// Used to render the widget in the UI, gets the widget for non generic widgets, otherwise fetches the model
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        object GetModel(Widget widget);

        void DeleteWidget(Widget widget);
        Widget AddWidget(Widget widget);
    }
}