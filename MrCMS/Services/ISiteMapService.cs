using System.Web.Mvc;

namespace MrCMS.Services
{
    public interface ISiteMapService
    {
        string GetSiteMap(UrlHelper urlHelper);
    }
}