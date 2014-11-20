using System.Collections.Generic;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using NHibernate;
using Ninject;

namespace MrCMS.Services.CloneSite
{
    public class CloneSiteService : ICloneSiteService
    {
        private readonly IKernel _kernel;
        private readonly ISession _session;

        public CloneSiteService(IKernel kernel, ISession session)
        {
            _kernel = kernel;
            _session = session;
        }

        public void CloneData(Site site, List<SiteCopyOption> options)
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
                    var @from = _session.Get<Site>(info.SiteId);
                    if (@from == null)
                        continue;
                    info.CloneSiteParts.Clone(@from, site, siteCloneContext);
                }
            }
        }

        private SiteCopyOptionInfo GetSiteCopyOptionInfo(SiteCopyOption option, IKernel kernel)
        {
            var cloneSiteParts = kernel.TryGet(option.SiteCopyActionType) as ICloneSiteParts;
            if (cloneSiteParts == null)
                return null;

            return new SiteCopyOptionInfo(option.SiteId, cloneSiteParts);
        }
    }
}