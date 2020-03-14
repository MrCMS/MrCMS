using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Models;

namespace MrCMS.Services.CloneSite
{
    public class CloneSiteService : ICloneSiteService
    {
        private readonly IServiceProvider _kernel;
        private readonly IGlobalRepository<Site> _repository;

        public CloneSiteService(IServiceProvider kernel, IGlobalRepository<Site> repository)
        {
            _kernel = kernel;
            _repository = repository;
        }

        public async Task CloneData(Site site, List<SiteCopyOption> options)
        {
            var siteCopyOptionInfos =
                options.Select(option => GetSiteCopyOptionInfo(option, _kernel))
                    .Where(info => info != null)
                    .OrderBy(x => x.Order);
            var siteCloneContext = new SiteCloneContext();
            foreach (var info in siteCopyOptionInfos)
            {
                var @from = _repository.LoadSync(info.SiteId);
                if (@from == null)
                    continue;
                await info.CloneSiteParts.Clone(@from, site, siteCloneContext);
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