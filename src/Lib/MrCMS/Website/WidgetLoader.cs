using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public class WidgetLoader : IWidgetLoader
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IRepository<Widget> _widgetRepository;
        private readonly IWidgetVisibilityChecker _widgetVisibilityChecker;
        private readonly IRepository<PageWidgetSort> _pageWidgetSortRepository;

        public WidgetLoader(IRepository<Webpage> webpageRepository, IRepository<Widget> widgetRepository, IWidgetVisibilityChecker widgetVisibilityChecker, IRepository<PageWidgetSort> pageWidgetSortRepository)
        {
            _webpageRepository = webpageRepository;
            _widgetRepository = widgetRepository;
            _widgetVisibilityChecker = widgetVisibilityChecker;
            _pageWidgetSortRepository = pageWidgetSortRepository;
        }
        public async Task<IList<Widget>> GetWidgets(LayoutArea area, Webpage webpage, bool showHidden = false)
        {
            var widgets = await _widgetRepository.Query().Where(x => x.LayoutAreaId == area.Id).ToListAsync();
            return await GetVisibleWidgets(area.Id, webpage, widgets, showHidden);
        }

        private async Task<List<Widget>> GetVisibleWidgets(int areaId, Webpage webpage, IReadOnlyList<Widget> allWidgets, bool showHidden = false)
        {
            var widgets = allWidgets.Where(widget => widget.WebpageId == null).ToList();

            if (webpage != null)
            {
                var page = webpage;

                widgets.AddRange(allWidgets.Where(widget => widget.WebpageId == page.Id));

                while ((page.ParentId) != null)
                {
                    page = await _webpageRepository.Load(page.ParentId.Value);

                    widgets.AddRange(
                        allWidgets.Where(widget => widget.WebpageId == page.Id && widget.IsRecursive));
                }

                var widgetsToRemove = new List<Widget>();

                if (!showHidden)
                {
                    foreach (var widget in widgets)
                    {
                        if (await _widgetVisibilityChecker.IsHidden(webpage, widget))
                            widgetsToRemove.Add(widget);
                    }
                }

                foreach (var widget in widgetsToRemove)
                {
                    widgets.Remove(widget);
                }

                var allSorts = await _pageWidgetSortRepository.Readonly().Where(x => x.WebpageId == webpage.Id).ToListAsync();

                var pageWidgetSorts =
                    allSorts.Where(sort => sort.LayoutAreaId == areaId).OrderBy(sort => sort.Order)
                        .ToList();

                if (pageWidgetSorts.Any())
                {
                    widgets =
                        widgets.OrderByDescending(
                                widget => pageWidgetSorts.Select(sort => sort.WidgetId).Contains(widget.Id)).ThenBy(
                                widget => pageWidgetSorts.Select(sort => sort.WidgetId).ToList().IndexOf(widget.Id))
                            .ToList();
                }
            }
            else
            {
                widgets.RemoveAll(widget => widget == null);
                widgets.Sort((widget1, widget2) => widget1.DisplayOrder - widget2.DisplayOrder);
            }

            return widgets;
        }

    }
}