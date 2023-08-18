using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class GetWebpageByUrl<T> : IGetWebpageByUrl<T> where T : Webpage
    {
        private readonly ISession _session;

        public GetWebpageByUrl(ISession session)
        {
            _session = session;
        }

        public async Task<T> GetByUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            /*var id = await GetId(url);
            if (!id.HasValue)
                return null;*/
            return await _session.Query<T>().Where(x=>x.UrlSegment == url).FirstOrDefaultAsync();
        }

        /*private async Task<int?> GetId(string url)
        {
            return await _session.Query<T>()
                .WithOptions(x => x.SetCacheable(true))
                .Where(doc => doc.UrlSegment == url)
                .Select(x => x.Id).FirstOrDefaultAsync();
        }*/
    }
}
