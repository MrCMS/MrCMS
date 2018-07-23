using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using NHibernate;

namespace MrCMS.Indexing
{
    public class OptimiseIndexes : SchedulableTask
    {
        private readonly IIndexService _indexService;
        private readonly ISession _session;
        private readonly ITriggerUrls _triggerUrls;

        public OptimiseIndexes(IIndexService indexService, ISession session, ITriggerUrls triggerUrls)
        {
            _indexService = indexService;
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
            var indexes = _indexService.GetIndexes();
            foreach (var site in sites)
            {
                var siteSettings = new SqlConfigurationProvider(_session, site).GetSiteSettings<SiteSettings>();

                _triggerUrls.Trigger(indexes.Select(index => string.Format("{0}/{1}?{2}={3}&type={4}",
                    site.GetFullDomain.TrimEnd('/'),
                    OptimiseIndexesController.OptimiseIndexUrl,
                    siteSettings.TaskExecutorKey,
                    siteSettings.TaskExecutorPassword, index.TypeName)));
            }
        }
    }
}