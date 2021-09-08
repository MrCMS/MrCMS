using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using ISession = NHibernate.ISession;

namespace MrCMS.Services
{
    public class WebpageUIService : IWebpageUIService
    {
        private readonly ISession _session;

        public WebpageUIService(ISession session)
        {
            _session = session;
        }

        public async Task<T> GetPage<T>(int id) where T : Webpage
        {
            return await _session.GetAsync<T>(id);
        }
    }
}