using Microsoft.AspNetCore.Http;

namespace MrCMS.Website
{
    public class GetSiteIdFromContext : GetSiteId, IGetSiteIdFromContext
    {
        public GetSiteIdFromContext(IHttpContextAccessor accessor) : base(accessor)
        {
            
        }
        int IGetSiteIdFromContext.GetId(HttpContext context) => GetSiteId.GetId(context);
    }
}