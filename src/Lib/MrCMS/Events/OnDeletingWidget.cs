using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;

namespace MrCMS.Events
{
    public class OnDeletingWidget : OnDataDeleting<Widget>
    {
        private readonly IJoinTableRepository<ShownWidget> _shownWidgetRepository;
        private readonly IJoinTableRepository<HiddenWidget> _hiddenWidgetRepository;

        public OnDeletingWidget(IJoinTableRepository<ShownWidget> shownWidgetRepository, IJoinTableRepository<HiddenWidget> hiddenWidgetRepository)
        {
            _shownWidgetRepository = shownWidgetRepository;
            _hiddenWidgetRepository = hiddenWidgetRepository;
        }

        public override async Task<IResult> OnDeleting(Widget entity, DbContext dbContext)
        {
            var shownWidgets =
                await _shownWidgetRepository.Query().Where(x => x.WidgetId == entity.Id).ToListAsync();
            if (shownWidgets.Any())
                await _shownWidgetRepository.DeleteRange(shownWidgets);
            var hiddenWidgets =
                await _hiddenWidgetRepository.Query().Where(x => x.WidgetId == entity.Id).ToListAsync();
            if (hiddenWidgets.Any())
                await _hiddenWidgetRepository.DeleteRange(hiddenWidgets);

            entity.LayoutArea?.Widgets.Remove(entity); //required to clear cache
            entity.Webpage?.Widgets.Remove(entity);

            return new Successful();
        }
    }
}