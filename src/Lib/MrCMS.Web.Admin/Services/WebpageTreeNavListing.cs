using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public abstract class WebpageTreeNavListing<T> : IWebpageTreeNavListing where T : Webpage
    {
        public abstract AdminTree GetTree(int? id);
        public abstract bool HasChildren(int id);
    }
}