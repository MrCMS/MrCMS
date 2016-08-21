using MrCMS.Data;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class WidgetService : IWidgetService
    {
        private readonly IRepository<Widget> _widgetRepository;

        public WidgetService(IRepository<Widget> widgetRepository)
        {
            _widgetRepository = widgetRepository;
        }

        public T GetWidget<T>(int id) where T : Widget
        {
            return _widgetRepository.Get<T>(id);
        }

        public void UpdateWidget(Widget widget)
        {
            _widgetRepository.Update(widget);
        }

        public void DeleteWidget(Widget widget)
        {
            _widgetRepository.Delete(widget);
        }

        public Widget AddWidget(Widget widget)
        {
            _widgetRepository.Add(widget);
            return widget;
        }
    }
}