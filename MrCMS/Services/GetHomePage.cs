using System.Linq;
using MrCMS.Entities.Documents.Web;
using NHibernate;

namespace MrCMS.Services
{
    public class GetHomePage : IGetHomePage
    {
        private readonly ISession _session;

        public GetHomePage(ISession session)
        {
            _session = session;
        }

        public Webpage Get()
        {
            return _session.QueryOver<Webpage>()
                .Where(document => document.Parent == null)
                .OrderBy(webpage => webpage.DisplayOrder).Asc
                .Cacheable().List()
                .FirstOrDefault(webpage => webpage.Published);
        }
    }
}