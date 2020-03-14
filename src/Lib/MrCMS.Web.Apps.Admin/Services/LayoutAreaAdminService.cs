using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Website;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class LayoutAreaAdminService : ILayoutAreaAdminService
    {
        private readonly IRepository<LayoutArea> _layoutAreaRepository;
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IRepository<Widget> _widgetRepository;
        private readonly IRepository<PageWidgetSort> _pageWidgetSortRepository;
        private readonly IWidgetLoader _widgetLoader;
        private readonly IMapper _mapper;

        public LayoutAreaAdminService(IRepository<LayoutArea> layoutAreaRepository, IRepository<Webpage> webpageRepository, IRepository<Widget> widgetRepository, IRepository<PageWidgetSort> pageWidgetSortRepository,
            IWidgetLoader widgetLoader,
            IMapper mapper)
        {
            _layoutAreaRepository = layoutAreaRepository;
            _webpageRepository = webpageRepository;
            _widgetRepository = widgetRepository;
            _pageWidgetSortRepository = pageWidgetSortRepository;
            _widgetLoader = widgetLoader;
            _mapper = mapper;
        }

        public AddLayoutAreaModel GetAddModel(int id)
        {
            return new AddLayoutAreaModel { LayoutId = id };
        }

        public async Task Add(AddLayoutAreaModel model)
        {
            var layoutArea = _mapper.Map<LayoutArea>(model);
            EnsureLayoutAreaIsSet(layoutArea);
            await _layoutAreaRepository.Add(layoutArea);
        }

        public UpdateLayoutAreaModel GetEditModel(int id)
        {
            return _mapper.Map<UpdateLayoutAreaModel>(GetArea(id));
        }

        public Layout GetLayout(int id)
        {
            return GetArea(id)?.Layout;
        }

        public async Task<IList<Widget>> GetWidgets(int id)
        {
            return await _widgetLoader.GetWidgets(GetArea(id), null);
        }

        private static void EnsureLayoutAreaIsSet(LayoutArea layoutArea)
        {
            if (layoutArea.Layout != null && layoutArea.Layout.LayoutAreas.Contains(layoutArea) == false)
                layoutArea.Layout.LayoutAreas.Add(layoutArea);
        }

        public async Task<LayoutArea> Update(UpdateLayoutAreaModel model)
        {
            var layoutArea = GetArea(model.Id);
            _mapper.Map(model, layoutArea);

            EnsureLayoutAreaIsSet(layoutArea);
            await _layoutAreaRepository.Update(layoutArea);
            return layoutArea;
        }

        public LayoutArea GetArea(int layoutAreaId)
        {
            return _layoutAreaRepository.LoadSync(layoutAreaId, x => x.Layout);
        }

        public async Task<LayoutArea> DeleteArea(int id)
        {
            var area = GetArea(id);

            if (area.Layout?.LayoutAreas.Contains(area) == true)
                area.Layout.LayoutAreas.Remove(area);
            await _layoutAreaRepository.Delete(area);

            return area;
        }

        public async Task SetWidgetOrders(PageWidgetSortModel pageWidgetSortModel)
        {

            var widgets = pageWidgetSortModel.Widgets.Select(model =>
              {
                  var widget = _widgetRepository.LoadSync(model.Id);
                  widget.DisplayOrder = model.Order;
                  return widget;
              }).ToList();

            await _widgetRepository.UpdateRange(widgets);
        }

        public async Task SetWidgetForPageOrders(PageWidgetSortModel pageWidgetSortModel)
        {

            var layoutArea = _layoutAreaRepository.LoadSync(pageWidgetSortModel.LayoutAreaId);
            var webpage = _webpageRepository.LoadSync(pageWidgetSortModel.WebpageId);
            foreach (var model in pageWidgetSortModel.Widgets)
            {
                var widget = _widgetRepository.LoadSync(model.Id);

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
                if (isNew)
                    await _pageWidgetSortRepository.Add(widgetSort);
                else
                    await _pageWidgetSortRepository.Update(widgetSort);
            }

        }


        public async Task ResetSorting(int id, int pageId)
        {
            var list = await _pageWidgetSortRepository.Query().Where(sort => sort.LayoutAreaId == id && sort.WebpageId == pageId).ToListAsync();

            await _pageWidgetSortRepository.DeleteRange(list);
        }

        public async Task<PageWidgetSortModel> GetSortModel(LayoutArea area, int? pageId)
        {
            var webpage = pageId.HasValue ? await _webpageRepository.Load(pageId.Value) : null;
            return new PageWidgetSortModel(await _widgetLoader.GetWidgets(area, webpage), area, webpage);
        }

    }
}