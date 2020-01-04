using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services.CloneSite;
using MrCMS.Web.Apps.Admin.Models;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class SiteAdminService : ISiteAdminService
    {
        private readonly ICloneSiteService _cloneSiteService;
        private readonly IMapper _mapper;
        private readonly ISession _session;

        public SiteAdminService(ISession session, ICloneSiteService cloneSiteService, IMapper mapper)
        {
            _session = session;
            _cloneSiteService = cloneSiteService;
            _mapper = mapper;
        }

        public List<Site> GetAllSites()
        {
            return _session.QueryOver<Site>().OrderBy(site => site.Name).Asc.Cacheable().List().ToList();
        }

        public Site GetSite(int id)
        {
            return _session.Get<Site>(id);
        }

        public void AddSite(AddSiteModel model, List<SiteCopyOption> options)
        {
            var site = _mapper.Map<Site>(model);

            _session.Transact(session => session.Save(site));

            _cloneSiteService.CloneData(site, options);
        }

        public UpdateSiteModel GetEditModel(int id)
        {
            var site =  _session.Get<Site>(id);

            return _mapper.Map<UpdateSiteModel>(site);
        }

        public IList<RedirectedDomain> GetRedirectedDomains(int id)
        {
            return _session.Query<RedirectedDomain>()
                .Where(x => x.Site.Id == id)
                .OrderBy(x => x.CreatedOn)
                .ToList();
        }

        public void SaveSite(UpdateSiteModel model)
        {
            var site = _session.Get<Site>(model.Id);
            _mapper.Map(model, site);
            _session.Transact(session => session.Update(site));
        }

        public void DeleteSite(int id)
        {
            var site = _session.Get<Site>(id);
            _session.Transact(session => session.Delete(site));
        }
    }
}