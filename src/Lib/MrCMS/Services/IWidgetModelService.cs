using MrCMS.Entities.Widget;

namespace MrCMS.Services
{
    public interface IWidgetModelService
    {
        /// <summary>
        /// Used to render the widget in the UI, gets the widget for non generic widgets, otherwise fetches the model
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        object GetModel(Widget widget);
    }
}