using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Admin.Services
{
    public class AdminSiteListService : IAdminSiteListService
    {
        private readonly ISession _session;
        private readonly ICurrentSiteLocator _siteLocator;

        public AdminSiteListService(ISession session, ICurrentSiteLocator siteLocator)
        {
            _session = session;
            _siteLocator = siteLocator;
        }

        public async Task<List<SelectListItem>> GetSiteOptions()
        {
            var sites = await GetSites();
            var currentSite = _siteLocator.GetCurrentSite();
            return sites.BuildSelectItemList(site => site.Name,
                site => string.Format((string)"https://{0}/admin/", (object)site.BaseUrl),
                site => site.Id == currentSite.Id,
                emptyItemText: null);
        }

        public async Task<IList<Site>> GetSites()
        {
            return await _session.QueryOver<Site>().OrderBy(x => x.Name).Asc.Cacheable().ListAsync();
        }
    }
}