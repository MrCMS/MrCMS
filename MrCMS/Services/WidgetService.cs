using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;
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

        public AddWidgetModel GetAddWidgetModel(int layoutAreaId, string returnUrl)
        {
            var types = WidgetHelper.WidgetTypes.BuildSelectItemList(type => type.Name.BreakUpString(),
                                                                     type => type.Name,
                                                                     emptyItem:
                                                                         SelectListItemHelper.EmptyItem("Select widget type..."));

            var addWidgetModel = new AddWidgetModel(types, layoutAreaId, returnUrl, "");
            return addWidgetModel;
        }

        public Widget AddWidget(int layoutAreaId, string widgetType, string name)
        {
            var newWidget = WidgetHelper.GetNewWidget(widgetType);
            var layoutArea = _session.Get<LayoutArea>(layoutAreaId);

            newWidget.LayoutArea = layoutArea;
            newWidget.Name = name;
            layoutArea.AddWidget(newWidget);

            _session.Transact(session =>
                                  {
                                      session.SaveOrUpdate(newWidget);
                                      session.SaveOrUpdate(layoutArea);
                                  });
            return newWidget;
            
        }

        public void DeleteWidget(int id)
        {
            var widget = GetWidget<Widget>(id);
            if (widget != null)
            {
                DeleteWidget(widget);
            }
        }
    }
}