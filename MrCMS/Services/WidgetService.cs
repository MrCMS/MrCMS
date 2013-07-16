using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class WidgetService : IWidgetService
    {
        private readonly ISession _session;

        public WidgetService(ISession session)
        {
            _session = session;
        }

        public T GetWidget<T>(int id) where T : Widget
        {
            return _session.Get<T>(id);
        }

        public void SaveWidget(Widget widget)
        {
            _session.Transact(session => session.SaveOrUpdate(widget));
        }

        public object GetModel(Widget widget)
        {
            return widget.GetModel(_session);
        }

        public void DeleteWidget(Widget widget)
        {
            _session.Transact(session =>
                {
                    widget.OnDeleting(session);
                    session.Delete(widget);
                });
        }

        public Widget AddWidget(Widget widget)
        {
            _session.Transact(session => session.SaveOrUpdate(widget));
            return widget;
        }
    }
}