using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Website;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class SiteService : ISiteService
    {
        private readonly ISession _session;
        private readonly HttpRequestBase _requestBase;
        private Site _currentSite;

        public SiteService(ISession session, HttpRequestBase requestBase)
        {
            _session = session;
            _requestBase = requestBase;
        }

        public List<Site> GetAllSites()
        {
            return _session.QueryOver<Site>().Cacheable().List().ToList();
        }

        public Site GetSite(int id)
        {
            return _session.Get<Site>(id);
        }

        public void AddSite(Site site)
        {
            _session.Transact(session =>
                                  {
                                      var user = CurrentRequestData.CurrentUser;

                                      if (user.Sites != null)
                                          user.Sites.Add(site);
                                      else
                                          user.Sites = new List<Site> { site };

                                      if (site.Users != null)
                                          site.Users.Add(user);
                                      else
                                          site.Users = new List<User> { user };

                                      session.Save(site);
                                      session.Update(user);
                                  });
        }

        public void SaveSite(Site site)
        {
            _session.Transact(session => session.Update(site));
        }

        public void DeleteSite(Site site)
        {
            _session.Transact(session =>
                                  {
                                      site.OnDeleting(session);
                                      session.Delete(site);
                                  });
        }

        public Site GetCurrentSite()
        {
            return _currentSite ?? (_currentSite = GetSiteFromSettingForDebugging() ?? GetSiteFromRequest());
        }

        private Site GetSiteFromSettingForDebugging()
        {
            var appSetting = ConfigurationManager.AppSettings["debugSiteId"];

            int id;
            return int.TryParse(appSetting, out id) ? _session.Get<Site>(id) : null;
        }

        private Site GetSiteFromRequest()
        {
            var authority = _requestBase.Url.Authority;

            var allSites = GetAllSites();
            var site = allSites.FirstOrDefault(s => s.BaseUrl.Equals(authority, StringComparison.OrdinalIgnoreCase));

            return site ?? allSites.First();
        }
    }
}