using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web.Mvc;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using ISession = NHibernate.ISession;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class LayoutAreaAdminService : ILayoutAreaAdminService
    {
        private readonly IRepository<LayoutArea> _layoutAreaRepository;
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IRepository<Widget> _widgetRepository;
        private readonly IRepository<PageWidgetSort> _pageWidgetSortRepository;

        public LayoutAreaAdminService(IRepository<LayoutArea> layoutAreaRepository, IRepository<Webpage> webpageRepository, IRepository<Widget> widgetRepository, IRepository<PageWidgetSort> pageWidgetSortRepository)
        {
            _layoutAreaRepository = layoutAreaRepository;
            _webpageRepository = webpageRepository;
            _widgetRepository = widgetRepository;
            _pageWidgetSortRepository = pageWidgetSortRepository;
        }

        public void Add(LayoutArea layoutArea)
        {
            EnsureLayoutAreaIsSet(layoutArea);
            _layoutAreaRepository.Add(layoutArea);
        }

        private static void EnsureLayoutAreaIsSet(LayoutArea layoutArea)
        {
            if (layoutArea.Layout != null && layoutArea.Layout.LayoutAreas.Contains(layoutArea) == false)
                layoutArea.Layout.LayoutAreas.Add(layoutArea);
        }

        public void Update(LayoutArea layoutArea)
        {
            EnsureLayoutAreaIsSet(layoutArea);
            _layoutAreaRepository.Update(layoutArea);
        }

        public LayoutArea GetArea(int layoutAreaId)
        {
            return _layoutAreaRepository.Get(layoutAreaId);
        }

        public void DeleteArea(LayoutArea area)
        {
            if (area.Layout?.LayoutAreas.Contains(area) == true)
                area.Layout.LayoutAreas.Remove(area);
            _layoutAreaRepository.Delete(area);
        }

        public void SetWidgetOrders(PageWidgetSortModel pageWidgetSortModel)
        {
            _widgetRepository.Transact(repository => pageWidgetSortModel.Widgets.ForEach(model =>
            {
                var widget = repository.Get(model.Id);
                widget.DisplayOrder = model.Order;
                repository.Update(widget);
            }));
        }

        public void SetWidgetForPageOrders(PageWidgetSortModel pageWidgetSortModel)
        {
            _layoutAreaRepository.Transact(layoutAreaRepository =>
            {

                var layoutArea = layoutAreaRepository.Get(pageWidgetSortModel.LayoutAreaId);
                var webpage = _webpageRepository.Get(pageWidgetSortModel.WebpageId);
                pageWidgetSortModel.Widgets.ForEach(model =>
                                                        {
                                                            var widget = _widgetRepository.Get(model.Id);

                                                            PageWidgetSort widgetSort = _pageWidgetSortRepository.Query()
                                                                .SingleOrDefault(
                                                                    sort => sort.LayoutArea == layoutArea &&
                                                                            sort.Webpage == webpage &&
                                                                            sort.Widget == widget);
                                                            var isNew = widgetSort == null;
                                                            if (isNew)
                                                                widgetSort = new PageWidgetSort
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
                                                            if (isNew)
                                                                _pageWidgetSortRepository.Add(widgetSort);
                                                            else
                                                                _pageWidgetSortRepository.Update(widgetSort);
                                                        });

            });
        }

        public void ResetSorting(LayoutArea area, int pageId)
        {
            var webpage = _webpageRepository.Get(pageId);
            var list = webpage.PageWidgetSorts.Where(sort => sort.LayoutArea == area).ToList();

            _pageWidgetSortRepository.Transact(repository => list.ForEach(repository.Delete));
        }

        public PageWidgetSortModel GetSortModel(LayoutArea area, int pageId)
        {
            var webpage = _webpageRepository.Get(pageId);
            return new PageWidgetSortModel(area.GetWidgets(webpage), area, webpage);
        }

    }
}