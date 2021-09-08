using System.Threading.Tasks;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IInPageAdminService
    {
        Task<SaveResult> SaveContent(UpdatePropertyData updatePropertyData);
        Task<ContentInfo> GetContent(GetPropertyData getPropertyData);
    }
}