using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using ISession = NHibernate.ISession;

namespace MrCMS.Tasks
{
    public class UpdateSitemap : SchedulableTask
    {
        private readonly ISession _session;
        private readonly ITriggerUrls _triggerUrls;
        private readonly IUrlHelper _urlHelper;

        public UpdateSitemap(ISession session,
            ITriggerUrls triggerUrls,
            IUrlHelper urlHelper)
        {
            _session = session;
            _triggerUrls = triggerUrls;
            _urlHelper = urlHelper;
        }

        public override int Priority
        {
            get { return 0; }
        }

        protected override void OnExecute()
        {
            var sites = _session.QueryOver<Site>().Where(x => !x.IsDeleted).List();

            _triggerUrls.Trigger(sites.Select(site =>
            {
                var siteSettings = new SqlConfigurationProvider(_session, site).GetSiteSettings<SiteSettings>();
                return _urlHelper.AbsoluteAction("Update", "Sitemap",
                    new RouteValueDictionary { [siteSettings.TaskExecutorKey] = siteSettings.TaskExecutorPassword }, site);
            }));
        }
    }
}