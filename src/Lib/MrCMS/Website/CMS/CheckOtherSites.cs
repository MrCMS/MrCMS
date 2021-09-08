using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website.NotFound;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Website.CMS
{
    public class CheckOtherSites : INotFoundRouteChecker
    {
        private readonly IServiceProvider _serviceProvider;

        public CheckOtherSites(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<NotFoundCheckResult> Check(string path, string query)
        {
            var session = _serviceProvider.GetRequiredService<ISession>();
            using var disabler = new SiteFilterDisabler(session);
            var url = path?.TrimStart('/');
            var webpage = await session.Query<Webpage>()
                .Where(doc => doc.UrlSegment == url)
                .Where(x => x.Published)
                .FirstOrDefaultAsync();

            if (webpage == null)
                return NotFoundCheckResult.NotFound;
            
            var getLiveUrl = _serviceProvider.GetRequiredService<IGetLiveUrl>();
            return NotFoundCheckResult.ForUrl(await getLiveUrl.GetAbsoluteUrl(webpage));
        }
    }
}