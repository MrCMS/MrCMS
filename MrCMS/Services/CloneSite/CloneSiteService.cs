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
        //private readonly ICloneSitePartsService _cloneSitePartsService;
        private readonly IKernel _kernel;
        private readonly ISession _session;

        public CloneSiteService(IKernel kernel, ISession session)
        {
            //_cloneSitePartsService = cloneSitePartsService;
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
                foreach (var info in siteCopyOptionInfos)
                {
                    var @from = _session.Get<Site>(info.SiteId);
                    if (@from == null)
                        continue;
                    info.CloneSiteParts.Clone(@from, site);
                }
                //if (!options.SiteId.HasValue)
                //    return;

                //var @from = _session.Get<Site>(options.SiteId.Value);
                //if (@from == null)
                //    return;
                //_cloneSitePartsService.CopySettings(@from, site);
                //if (options.CopyLayouts)
                //    _cloneSitePartsService.CopyLayouts(@from, site);
                //if (options.CopyMediaCategories)
                //    _cloneSitePartsService.CopyMediaCategories(@from, site);
                //if (options.CopyHome)
                //    _cloneSitePartsService.CopyHome(@from, site);
                //if (options.Copy404)
                //    _cloneSitePartsService.Copy404(@from, site);
                //if (options.Copy403)
                //    _cloneSitePartsService.Copy403(@from, site);
                //if (options.Copy500)
                //    _cloneSitePartsService.Copy500(@from, site);
                //if (options.CopyLogin)
                //    _cloneSitePartsService.CopyLogin(@from, site);
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