using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MrCMS.Web.Admin.Services
{
    public interface IUserAvatarService
    {
        Task SetAvatar(int userId, IFormFile formFile);
    }
}