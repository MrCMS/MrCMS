using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MrCMS.Data;
using MrCMS.Entities.Multisite;

namespace MrCMS.Website
{
    public interface ISetSiteId
    {
        void SetId(HttpContext context);
    }

    public class SetSiteId : ISetSiteId
    {
        private readonly IOptions<SiteDebugOptions> _siteDebugOptions;

        public SetSiteId(IOptions<SiteDebugOptions> siteDebugOptions)
        {
            _siteDebugOptions = siteDebugOptions;
        }

        public void SetId(HttpContext context)
        {
            context.Items[CurrentSiteMiddleware.SiteIdKey] = GetId(context);
        }

        private int GetId(HttpContext context)
        {
            var debugSiteId = _siteDebugOptions?.Value?.SiteId;
            var repository = context.RequestServices.GetRequiredService<IGlobalRepository<Site>>();
            if (debugSiteId.HasValue)
            {
                var debugSite = repository.Readonly().FirstOrDefault(x => x.Id == debugSiteId.Value);
                if (debugSite != null)
                    return debugSite.Id;
            }
            var request = context.Request;
            if (request != null)
            {
                var host = request.Host;
                var component = host.ToUriComponent();

                var site = repository.Readonly().FirstOrDefault(x => x.BaseUrl == component);

                if (site != null)
                    return site.Id;
            }

            return repository.Readonly().FirstOrDefault()?.Id ?? 1;
        }
    }
    public class SiteDebugOptions
    {
        public int? SiteId { get; set; }
    }

    public class CurrentSiteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISetSiteId _setSiteId;
        public const string SiteIdKey = "current-site-id";

        public CurrentSiteMiddleware(RequestDelegate next, ISetSiteId setSiteId)
        {
            _next = next;
            _setSiteId = setSiteId;
        }

        /// <summary>
        /// Processes a request to determine if it matches a known file, and if so, serves it.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext context)
        {
            _setSiteId.SetId(context);
            return _next(context);
        }
    }
}