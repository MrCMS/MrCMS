using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Tasks
{
    public class UpdateSitemap : SchedulableTask
    {
        private readonly IStatelessSession _session;
        private readonly ITriggerUrls _triggerUrls;


        public UpdateSitemap(IStatelessSession session, ITriggerUrls triggerUrls)
        {
            _session = session;
            _triggerUrls = triggerUrls;
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
                return string.Format("{0}/{1}?{2}={3}",
                    site.GetFullDomain.TrimEnd('/'),
                    SitemapController.WriteSitemapUrl,
                    siteSettings.TaskExecutorKey,
                    siteSettings.TaskExecutorPassword);
            }));
        }
    }
}