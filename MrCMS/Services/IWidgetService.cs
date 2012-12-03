using MrCMS.Entities.Widget;
using MrCMS.Models;

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
        AddWidgetModel GetAddWidgetModel(int layoutAreaId, string returnUrl);
        Widget AddWidget(int layoutAreaId, string widgetType, string name);
    }
}