using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class GetCurrentSite : IGetCurrentSite
    {
        private readonly IGetSiteId _getSiteId;
        private readonly IGlobalRepository<Site> _repository;

        public GetCurrentSite(IGetSiteId getSiteId, IGlobalRepository<Site> repository)
        {
            _getSiteId = getSiteId;
            _repository = repository;
        }
        public Task<Site> GetSite()
        {
            var siteId = _getSiteId.GetId();

            return _repository.Load(siteId);
        }
    }
}