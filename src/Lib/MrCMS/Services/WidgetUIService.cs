using MrCMS.Entities.Widget;
using NHibernate;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class WidgetUIService : IWidgetUIService
    {
        private readonly ISession _session;

        public WidgetUIService(ISession session)
        {
            _session = session;
        }

        public async Task<T> GetWidgetAsync<T>(int id, CancellationToken token = default) where T : Widget
        {
            return (await _session.GetAsync<T>(id, token)).Unproxy(); 
        }
    }
}