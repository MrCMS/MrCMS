using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IInPageAdminService
    {
        Task<SaveResult> SaveContent(UpdatePropertyData updatePropertyData);
        Task<ContentInfo> GetContent(GetPropertyData getPropertyData);
    }
}