using Microsoft.AspNetCore.Http;

namespace MrCMS.Website
{
    public interface IGetSiteIdFromContext : IGetSiteId
    {
        int GetId(HttpContext context);
    }
}