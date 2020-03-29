using System.Threading.Tasks;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IInPageAdminService
    {
        Task<SaveResult> SaveContent(UpdatePropertyData updatePropertyData);
        Task<ContentInfo> GetContent(GetPropertyData getPropertyData);
    }
}