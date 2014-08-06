using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class LayoutAreaAdminService : ILayoutAreaAdminService
    {
        private readonly ISession _session;

        public LayoutAreaAdminService(ISession session)
        {
            _session = session;
        }
        public LayoutArea GetArea(Layout layout, string name)
        {
            return _session.QueryOver<LayoutArea>().Where(x => x.Layout == layout && x.AreaName == name).Fetch(
                area => area.Widgets).Eager.TransformUsing(Transformers.DistinctRootEntity).SingleOrDefault();
        }

        public void SaveArea(LayoutArea layoutArea)
        {
            _session.Transact(session =>
                                  {
                                      var layout = layoutArea.Layout;
                                      if (layout != null)
                                      {
                                          layout.LayoutAreas.Add(layoutArea);
                                      }
                                      session.SaveOrUpdate(layoutArea);
                                  });
        }

        public LayoutArea GetArea(int layoutAreaId)
        {
            return _session.Get<LayoutArea>(layoutAreaId);
        }

        public void DeleteArea(LayoutArea area)
        {
            _session.Transact(session =>
                                  {
                                      if (area.Layout != null)
                                          area.Layout.LayoutAreas.Remove(area); //required to clear cache
                                      session.Delete(area);
                                  });
        }

        public void SetWidgetOrders(PageWidgetSortModel pageWidgetSortModel)
        {
            _session.Transact(session => pageWidgetSortModel.Widgets.ForEach(model =>
                                                                                 {
                                                                                     var widget = _session.Get<Widget>(model.Id);
                                                                                     widget.DisplayOrder = model.Order;
                                                                                     session.SaveOrUpdate(widget);
                                                                                 }));
        }

        public void SetWidgetForPageOrders(PageWidgetSortModel pageWidgetSortModel)
        {
            _session.Transact(session =>
            {

                var layoutArea = _session.Get<LayoutArea>(pageWidgetSortModel.LayoutAreaId);
                var webpage = _session.Get<Webpage>(pageWidgetSortModel.WebpageId);
                pageWidgetSortModel.Widgets.ForEach(model =>
                                                        {
                                                            var widget = _session.Get<Widget>(model.Id);

                                                            var widgetSort =
                                                                _session.QueryOver<PageWidgetSort>().Where(
                                                                    sort =>
                                                                    sort.LayoutArea == layoutArea &&
                                                                    sort.Webpage == webpage &&
                                                                    sort.Widget == widget).SingleOrDefault() ??
                                                                new PageWidgetSort
                                                                    {
                                                                        LayoutArea =
                                                                            layoutArea,
                                                                        Webpage = webpage,
                                                                        Widget = widget
                                                                    };
                                                            widgetSort.Order = model.Order;
                                                            if (!layoutArea.PageWidgetSorts.Contains(widgetSort))
                                                                layoutArea.PageWidgetSorts.Add(widgetSort);
                                                            if (!webpage.PageWidgetSorts.Contains(widgetSort))
                                                                webpage.PageWidgetSorts.Add(widgetSort);
                                                            if (!widget.PageWidgetSorts.Contains(widgetSort))
                                                                widget.PageWidgetSorts.Add(widgetSort);
                                                            session.SaveOrUpdate(widgetSort);
                                                        });

            });
        }

        public void ResetSorting(LayoutArea area, int pageId)
        {
            var webpage = _session.Get<Webpage>(pageId);
            var list = webpage.PageWidgetSorts.Where(sort => sort.LayoutArea == area).ToList();

            _session.Transact(session => list.ForEach(session.Delete));
        }

        public PageWidgetSortModel GetSortModel(LayoutArea area, int pageId)
        {
            var webpage = _session.Get<Webpage>(pageId);
            return new PageWidgetSortModel(area.GetWidgets(webpage), area, webpage);
        }
    }
}