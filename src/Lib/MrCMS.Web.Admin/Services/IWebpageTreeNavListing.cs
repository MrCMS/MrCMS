using System.Threading.Tasks;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IWebpageTreeNavListing
    {
        Task<AdminTree> GetTree(int? id);
        Task<bool> HasChildren(int id);
    }
}