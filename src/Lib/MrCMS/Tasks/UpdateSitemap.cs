using System.Threading.Tasks;
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

        protected override async Task OnExecute()
        {
            await _sitemapService.WriteSitemap();
        }
    }
}