using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Web.Admin.Services
{
    public class RedirectedDomainService : IRedirectedDomainService
    {
        private readonly ISession _session;

        public RedirectedDomainService(ISession session)
        {
            _session = session;
        }

        public async Task Save(RedirectedDomain domain)
        {
            if (domain.Site != null)
                domain.Site.RedirectedDomains.Add(domain);
            await _session.TransactAsync(session => session.SaveAsync(domain));
        }

        public async Task Delete(int id)
        {
            var rd = _session.Get<RedirectedDomain>(id);
            rd.Site.RedirectedDomains.Remove(rd);
            
            await _session.TransactAsync(session =>  session.DeleteAsync(rd));
        }
    }
}