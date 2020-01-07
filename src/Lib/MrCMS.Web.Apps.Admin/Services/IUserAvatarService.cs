using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IUserAvatarService
    {
        Task SetAvatar(int userId, IFormFile formFile);
    }
}