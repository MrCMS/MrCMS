using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Website.NotFound;

namespace MrCMS.Website.CMS
{
    public class AppendTrailingSlashCheck : INotFoundRouteChecker
    {
        private readonly IServiceProvider _serviceProvider;

        public AppendTrailingSlashCheck(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public async Task<NotFoundCheckResult> Check(string path, string query)
        {
            if (!string.IsNullOrWhiteSpace(path) && !path.EndsWith('/'))
            {
                var getWebpageForPath = _serviceProvider.GetRequiredService<IGetWebpageForPath>();
                var webpage = await getWebpageForPath.GetWebpage(path + '/');
                if (webpage != null)
                {
                    return NotFoundCheckResult.ForWebpage(webpage);
                }
            }

            return NotFoundCheckResult.NotFound;
        }
    }
}