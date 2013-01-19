using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface INavigationService
    {
        SiteTree<Webpage> GetWebsiteTree(int? depth = null);
        SiteTree<MediaCategory> GetMediaTree();
        SiteTree<Layout> GetLayoutList();
        SiteTree<User> GetUserList();
        IEnumerable<SelectListItem> GetParentsList();
        string GetSiteMap(UrlHelper urlHelper);
        IEnumerable<SelectListItem> GetDocumentTypes(string type);
    }
}