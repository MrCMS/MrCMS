using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IUserAvatarService
    {
        Task SetAvatar(int userId, IFormFile formFile);
    }
}