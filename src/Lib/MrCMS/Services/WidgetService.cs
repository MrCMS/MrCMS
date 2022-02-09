using System.Threading.Tasks;
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

        public Task<T> GetWidget<T>(int id) where T : Widget
        {
            return _session.GetAsync<T>(id);
        }

        public async Task SaveWidget(Widget widget)
        {
            await _session.TransactAsync(session => session.SaveOrUpdateAsync(widget));
        }

        public async Task DeleteWidget(Widget widget)
        {
            await _session.TransactAsync(session => session.DeleteAsync(widget));
        }

        public async Task<Widget> AddWidget(Widget widget)
        {
            await _session.TransactAsync(session => session.SaveOrUpdateAsync(widget));
            return widget;
        }
    }
}