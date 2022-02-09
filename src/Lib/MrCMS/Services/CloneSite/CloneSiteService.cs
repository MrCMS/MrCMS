using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    public class CloneSiteService : ICloneSiteService
    {
        private readonly IServiceProvider _kernel;
        private readonly ISession _session;

        public CloneSiteService(IServiceProvider kernel, ISession session)
        {
            _kernel = kernel;
            _session = session;
        }

        public async Task CloneData(Site site, List<SiteCopyOption> options)
        {
            using (new SiteFilterDisabler(_session))
            {
                var siteCopyOptionInfos =
                    options.Select(option => GetSiteCopyOptionInfo(option, _kernel))
                        .Where(info => info != null)
                        .OrderBy(x => x.Order);
                var siteCloneContext = new SiteCloneContext();
                foreach (var info in siteCopyOptionInfos)
                {
                    var @from = await _session.GetAsync<Site>(info.SiteId);
                    if (@from == null)
                        continue;
                    await info.CloneSiteParts.Clone(@from, site, siteCloneContext);
                }
            }
        }

        private SiteCopyOptionInfo GetSiteCopyOptionInfo(SiteCopyOption option, IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(option.SiteCopyActionType) is ICloneSiteParts cloneSiteParts))
                return null;

            return new SiteCopyOptionInfo(option.SiteId, cloneSiteParts);
        }
    }
}