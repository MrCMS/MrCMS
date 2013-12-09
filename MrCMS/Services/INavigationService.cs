using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface INavigationService
    {
        SiteTree<Webpage> GetWebsiteTree(int? depth = null);
        SiteTree<MediaCategory> GetMediaTree();
        SiteTree<Layout> GetLayoutList();
        IEnumerable<SelectListItem> GetParentsList();
        string GetSiteMap(UrlHelper urlHelper);
        IEnumerable<SelectListItem> GetDocumentTypes(string type);
        IEnumerable<IAdminMenuItem> GetNavLinks();
    }
}