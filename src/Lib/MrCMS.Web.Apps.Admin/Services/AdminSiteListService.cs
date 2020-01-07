using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class AdminSiteListService : IAdminSiteListService
    {
        private readonly IGlobalRepository<Site> _repository;
        private readonly Site _site;

        public AdminSiteListService(IGlobalRepository<Site> repository, Site site)
        {
            _repository = repository;
            _site = site;
        }

        public List<SelectListItem> GetSiteOptions()
        {
            return GetSites().BuildSelectItemList(site => site.Name, site => string.Format((string)"http://{0}/admin/", (object)site.BaseUrl),
                                             site => site.Id == _site.Id,
                                             emptyItemText: null);
        }

        public IList<Site> GetSites()
        {
            return _repository.Query().OrderBy(x => x.Name).ToList();
        }
    }
}