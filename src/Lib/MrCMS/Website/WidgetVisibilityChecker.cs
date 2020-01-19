using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;

namespace MrCMS.Website
{
    public class WidgetVisibilityChecker : IWidgetVisibilityChecker
    {
        private readonly IActivePagesLoader _activePagesLoader;
        private readonly IJoinTableRepository<ShownWidget> _shownWidgetRepository;
        private readonly IJoinTableRepository<HiddenWidget> _hiddenWidgetRepository;

        public WidgetVisibilityChecker(IActivePagesLoader activePagesLoader, IJoinTableRepository<ShownWidget> shownWidgetRepository, IJoinTableRepository<HiddenWidget> hiddenWidgetRepository)
        {
            _activePagesLoader = activePagesLoader;
            _shownWidgetRepository = shownWidgetRepository;
            _hiddenWidgetRepository = hiddenWidgetRepository;
        }

        public async Task<bool> IsHidden(Webpage webpage, Widget widget)
        {
            if (widget.WebpageId == webpage.Id)
                return false;

            foreach (Webpage item in await _activePagesLoader.GetActivePages(webpage))
            {
                if (await _hiddenWidgetRepository.Readonly().AnyAsync(x=>x.WebpageId == webpage.Id && x.WidgetId == widget.Id))
                    return true;
                if (await _shownWidgetRepository.Readonly().AnyAsync(x=>x.WebpageId == webpage.Id && x.WidgetId == widget.Id))
                    return false;

                // if it's not overidden somehow and it is from the item we're looking at, use the recursive flag from the widget
                if (widget.WebpageId == item.Id)
                    return !widget.IsRecursive;
            }
            return false;
        }

        public async Task<int> CountVisible(IEnumerable<Widget> widgets, Webpage webpage)
        {
            int count = 0;

            foreach (var widget in widgets)
            {
                if (!await IsHidden(webpage, widget))
                    count++;
            }

            return count;
        }

        public async Task<int> CountHidden(IEnumerable<Widget> widgets, Webpage webpage)
        {
            return widgets.Count() - await CountVisible(widgets, webpage);
        }
    }
}