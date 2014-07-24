using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public abstract class WebpageTreeNavListing<T> : IWebpageTreeNavListing where T : Webpage
    {
        public abstract AdminTree GetTree(int? id);
    }
}