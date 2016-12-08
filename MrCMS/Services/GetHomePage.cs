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
                .Where(document => document.Parent == null && document.Published)
                .OrderBy(webpage => webpage.DisplayOrder).Asc
                .Take(1)
                .Cacheable()
                .List()
                .FirstOrDefault();
        }
    }
}