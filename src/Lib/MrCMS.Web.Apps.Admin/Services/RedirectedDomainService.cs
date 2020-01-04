using MrCMS.Entities.Multisite;
using MrCMS.Helpers;


namespace MrCMS.Web.Apps.Admin.Services
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
            if (domain.Site != null)
                domain.Site.RedirectedDomains.Add(domain);
            _session.Transact(session => session.Save(domain));
        }

        public void Delete(int id)
        {
            var rd = _session.Get<RedirectedDomain>(id);
            rd.Site.RedirectedDomains.Remove(rd);
            
            _session.Transact(session => session.Delete(rd));
        }
    }
}