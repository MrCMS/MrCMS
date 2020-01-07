using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class RedirectedDomainService : IRedirectedDomainService
    {
        private readonly IGlobalRepository<RedirectedDomain> _repository;
        //private readonly ISession _session;

        public RedirectedDomainService(IGlobalRepository<RedirectedDomain> repository)
        {
            _repository = repository;
        }

        public async Task Save(RedirectedDomain domain)
        {
            domain.Site?.RedirectedDomains.Add(domain);
            await _repository.Add(domain);
        }

        public async Task Delete(int id)
        {
            var rd =await _repository.Load(id);
            rd.Site.RedirectedDomains.Remove(rd);

            await _repository.Delete(rd);
        }
    }
}