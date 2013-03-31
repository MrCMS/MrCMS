using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class UrlHistoryService : IUrlHistoryService
    {
        private readonly ISession _session;

        public UrlHistoryService (ISession session)
        {
            _session = session;
        }

        public void Delete(UrlHistory urlHistory)
        {
            _session.Transact(session => _session.Delete(urlHistory));
        }

        public void Add(UrlHistory urlHistory)
        {
            _session.Transact(session => session.Save(urlHistory));
        }    
    }
}