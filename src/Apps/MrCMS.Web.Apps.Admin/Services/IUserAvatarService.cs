using Microsoft.AspNetCore.Http;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IUserAvatarService
    {
        void SetAvatar(int userId, IFormFile formFile);
    }
}