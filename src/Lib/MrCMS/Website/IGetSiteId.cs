using Microsoft.AspNetCore.Http;

namespace MrCMS.Website
{
    public interface IGetSiteId
    {
        int GetId();
        int GetId(HttpContext context);
    }
}