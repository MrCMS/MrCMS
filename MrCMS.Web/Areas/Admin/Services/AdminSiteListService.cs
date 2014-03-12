using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class AdminSiteListService : IAdminSiteListService
    {
        private readonly ISession _session;
        private readonly Site _site;

        public AdminSiteListService(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public List<SelectListItem> GetSiteOptions()
        {
            var sites = _session.QueryOver<Site>().Cacheable().List();

            return sites.BuildSelectItemList(site => site.Name, site => string.Format((string) "http://{0}/admin/", (object) site.BaseUrl),
                                             site => site.Id == _site.Id,
                                             emptyItemText: null);
        }
    }
}