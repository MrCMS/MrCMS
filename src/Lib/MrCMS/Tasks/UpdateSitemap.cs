using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Services.CloneSite;
using MrCMS.Settings;

namespace MrCMS.Tasks
{
    public class UpdateSitemap : SchedulableTask
    {
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly ITriggerUrls _triggerUrls;
        private readonly IUrlHelper _urlHelper;

        public UpdateSitemap(
            IRepositoryResolver repositoryResolver,
            ITriggerUrls triggerUrls,
            IUrlHelper urlHelper)
        {
            _repositoryResolver = repositoryResolver;
            _triggerUrls = triggerUrls;
            _urlHelper = urlHelper;
        }

        public override int Priority
        {
            get { return 0; }
        }

        protected override async Task OnExecute(CancellationToken token)
        {
            var sites = await _repositoryResolver.GetGlobalRepository<Site>().Query().Where(x => !x.IsDeleted)
                .ToListAsync(token);

            var urls = new List<string>();
            foreach (var site in sites)
            {

                var siteSettings = await new SqlConfigurationProvider(_repositoryResolver.GetGlobalRepository<Setting>(), SiteId.GetForSite(site), new NullEventContext()).GetSiteSettings<SiteSettings>();
                urls.Add(_urlHelper.AbsoluteAction("Update", "Sitemap",
                    new RouteValueDictionary
                    { [siteSettings.TaskExecutorKey] = siteSettings.TaskExecutorPassword }, site));
            }
            _triggerUrls.Trigger(urls);
        }
    }
}