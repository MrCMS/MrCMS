using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services.CloneSite;
using MrCMS.Web.Apps.Admin.Models;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class SiteAdminService : ISiteAdminService
    {
        private readonly IGlobalRepository<Site> _repository;
        private readonly IGlobalRepository<RedirectedDomain> _redirectedDomainRepository;
        private readonly ICloneSiteService _cloneSiteService;
        private readonly IMapper _mapper;

        public SiteAdminService(IGlobalRepository<Site> repository, IGlobalRepository<RedirectedDomain> redirectedDomainRepository, ICloneSiteService cloneSiteService, IMapper mapper)
        {
            _repository = repository;
            _redirectedDomainRepository = redirectedDomainRepository;
            _cloneSiteService = cloneSiteService;
            _mapper = mapper;
        }

        public List<Site> GetAllSites()
        {
            return _repository.Query().ToList();
        }

        public Site GetSite(int id)
        {
            return _repository.LoadSync(id);
        }

        public async Task AddSite(AddSiteModel model, List<SiteCopyOption> options)
        {
            var site = _mapper.Map<Site>(model);

            await _repository.Add(site);

            _cloneSiteService.CloneData(site, options);
        }

        public UpdateSiteModel GetEditModel(int id)
        {
            var site =  _repository.GetDataSync(id);

            return _mapper.Map<UpdateSiteModel>(site);
        }

        public IList<RedirectedDomain> GetRedirectedDomains(int id)
        {
            return _redirectedDomainRepository.Query()
                .Where(x => x.Site.Id == id)
                .OrderBy(x => x.CreatedOn)
                .ToList();
        }

        public async Task SaveSite(UpdateSiteModel model)
        {
            var site = await _repository.Load(model.Id);
            _mapper.Map(model, site);
            await _repository.Update(site);
        }

        public async Task DeleteSite(int id)
        {
            var site = await _repository.Load(id);
            await _repository.Delete(site);
        }
    }
}