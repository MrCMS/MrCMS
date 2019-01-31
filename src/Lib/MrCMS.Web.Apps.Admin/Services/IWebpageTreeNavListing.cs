using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IWebpageTreeNavListing
    {
        AdminTree GetTree(int? id);
        bool HasChildren(int id);
    }
}