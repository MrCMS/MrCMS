using MrCMS.Services.Sitemaps;

namespace MrCMS.Tasks
{
    public class UpdateSitemap : SchedulableTask
    {
        private readonly ISitemapService _sitemapService;

        public UpdateSitemap(ISitemapService sitemapService)
        {
            _sitemapService = sitemapService;
        }

        public override int Priority
        {
            get { return 0; }
        }

        protected override void OnExecute()
        {
            _sitemapService.WriteSitemap();
        }
    }
}