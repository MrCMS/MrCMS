using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public abstract class WebpageTreeNavListing<T> : IWebpageTreeNavListing where T : Webpage
    {
        public abstract AdminTree GetTree(int? id);
        public abstract bool HasChildren(int id);
    }
}