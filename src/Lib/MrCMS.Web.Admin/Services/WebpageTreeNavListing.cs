using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public abstract class WebpageTreeNavListing<T> : IWebpageTreeNavListing where T : Webpage
    {
        public abstract Task<AdminTree> GetTree(int? id);
        public abstract Task<bool> HasChildren(int id);
    }
}