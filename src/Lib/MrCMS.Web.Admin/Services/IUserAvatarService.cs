using Microsoft.AspNetCore.Http;

namespace MrCMS.Web.Admin.Services
{
    public interface IUserAvatarService
    {
        void SetAvatar(int userId, IFormFile formFile);
    }
}