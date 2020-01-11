using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class WebpageWidgetAdminService : IWebpageWidgetAdminService
    {
        private readonly IJoinTableRepository<HiddenWidget> _hiddenWidgetRepository;
        private readonly IJoinTableRepository<ShownWidget> _shownWidgetRepository;


        public WebpageWidgetAdminService(IJoinTableRepository<HiddenWidget> hiddenWidgetRepository, IJoinTableRepository<ShownWidget> shownWidgetRepository)
        {
            _hiddenWidgetRepository = hiddenWidgetRepository;
            _shownWidgetRepository = shownWidgetRepository;
        }

        public async Task Hide(int webpageId, int widgetId)
        {
            var shownWidgets = await _shownWidgetRepository.Query()
                .Where(x => x.WebpageId == webpageId && x.WidgetId == widgetId).ToListAsync();
            if (shownWidgets.Any())
                await _shownWidgetRepository.DeleteRange(shownWidgets);
            await _hiddenWidgetRepository.Add(new HiddenWidget { WebpageId = webpageId, WidgetId = widgetId });
        }

        public async Task Show(int webpageId, int widgetId)
        {
            var hiddenWidgets = await _hiddenWidgetRepository.Query()
                .Where(x => x.WebpageId == webpageId && x.WidgetId == widgetId).ToListAsync();
            if (hiddenWidgets.Any())
                await _hiddenWidgetRepository.DeleteRange(hiddenWidgets);
            await _shownWidgetRepository.Add(new ShownWidget() { WebpageId = webpageId, WidgetId = widgetId });
        }
    }
}