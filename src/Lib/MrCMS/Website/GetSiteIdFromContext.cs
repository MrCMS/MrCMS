using Microsoft.AspNetCore.Http;

namespace MrCMS.Website
{
    public class GetSiteIdFromContext : IGetSiteId
    {
        private readonly IHttpContextAccessor _accessor;

        public GetSiteIdFromContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public int GetId()
        {
            return GetId(_accessor.HttpContext);
        }

        int IGetSiteId.GetId(HttpContext context)
        {
            return GetId(context);
        }

        public static int GetId(HttpContext context)
        {
            return context.Items[CurrentSiteMiddleware.SiteIdKey] as int? ?? 1;
        }
    }
}