using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IWebpageTreeNavListing
    {
        AdminTree GetTree(int? id);
        bool HasChildren(int id);
    }
}