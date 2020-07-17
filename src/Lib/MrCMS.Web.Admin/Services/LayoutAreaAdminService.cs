using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public class LayoutAreaAdminService : ILayoutAreaAdminService
    {
        private readonly IRepository<LayoutArea> _layoutAreaRepository;
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IRepository<Widget> _widgetRepository;
        private readonly IRepository<PageWidgetSort> _pageWidgetSortRepository;
        private readonly IMapper _mapper;

        public LayoutAreaAdminService(IRepository<LayoutArea> layoutAreaRepository, IRepository<Webpage> webpageRepository, IRepository<Widget> widgetRepository, IRepository<PageWidgetSort> pageWidgetSortRepository,
            IMapper mapper)
        {
            _layoutAreaRepository = layoutAreaRepository;
            _webpageRepository = webpageRepository;
            _widgetRepository = widgetRepository;
            _pageWidgetSortRepository = pageWidgetSortRepository;
            _mapper = mapper;
        }

        public AddLayoutAreaModel GetAddModel(int id)
        {
            return new AddLayoutAreaModel { LayoutId = id };
        }

        public void Add(AddLayoutAreaModel model)
        {
            var layoutArea = _mapper.Map<LayoutArea>(model);
            EnsureLayoutAreaIsSet(layoutArea);
            _layoutAreaRepository.Add(layoutArea);
        }

        public UpdateLayoutAreaModel GetEditModel(int id)
        {
            return _mapper.Map<UpdateLayoutAreaModel>(GetArea(id));
        }

        public Layout GetLayout(int id)
        {
            return GetArea(id)?.Layout.Unproxy();
        }

        public IList<Widget> GetWidgets(int id)
        {
            return GetArea(id)?.GetWidgets();
        }

        private static void EnsureLayoutAreaIsSet(LayoutArea layoutArea)
        {
            if (layoutArea.Layout != null && layoutArea.Layout.LayoutAreas.Contains(layoutArea) == false)
                layoutArea.Layout.LayoutAreas.Add(layoutArea);
        }

        public LayoutArea Update(UpdateLayoutAreaModel model)
        {
            var layoutArea = GetArea(model.Id);
            _mapper.Map(model, layoutArea);

            EnsureLayoutAreaIsSet(layoutArea);
            _layoutAreaRepository.Update(layoutArea);
            return layoutArea;
        }

        public LayoutArea GetArea(int layoutAreaId)
        {
            return _layoutAreaRepository.Get(layoutAreaId);
        }

        public LayoutArea DeleteArea(int id)
        {
            var area = GetArea(id);

            if (area.Layout?.LayoutAreas.Contains(area) == true)
                area.Layout.LayoutAreas.Remove(area);
            _layoutAreaRepository.Delete(area);

            return area;
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

        public void ResetSorting(int id, int pageId)
        {
            var webpage = _webpageRepository.Get(pageId);
            var list = webpage.PageWidgetSorts.Where(sort => sort.LayoutArea?.Id == id).ToList();

            _pageWidgetSortRepository.Transact(repository => list.ForEach(repository.Delete));
        }

        public PageWidgetSortModel GetSortModel(LayoutArea area, int pageId)
        {
            var webpage = _webpageRepository.Get(pageId);
            return new PageWidgetSortModel(area.GetWidgets(webpage), area, webpage);
        }

    }
}