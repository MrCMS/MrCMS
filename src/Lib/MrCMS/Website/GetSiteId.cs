using Microsoft.AspNetCore.Http;

namespace MrCMS.Website
{
    public class GetSiteId : IGetSiteId
    {
        private readonly IHttpContextAccessor _accessor;

        public GetSiteId(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public int GetId()
        {
            return GetId(_accessor.HttpContext);
        }


        public static int GetId(HttpContext context)
        {
            return context.Items[CurrentSiteMiddleware.SiteIdKey] as int? ?? 1;
        }
    }
}