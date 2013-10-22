using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class RedirectedDomainService : IRedirectedDomainService
    {
        private readonly ISession _session;

        public RedirectedDomainService(ISession session)
        {
            _session = session;
        }

        public void Save(RedirectedDomain domain)
        {
            _session.Transact(session => session.Save(domain));
        }

        public void Delete(RedirectedDomain domain)
        {
            if (domain.Site != null)
                domain.Site.RedirectedDomains.Remove(domain);
            _session.Transact(session => session.Delete(domain));
        }
    }
}