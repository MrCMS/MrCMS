using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IWebpageTreeNavListing
    {
        AdminTree GetTree(int? id);
        bool HasChildren(int id);
    }
}