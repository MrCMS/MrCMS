using System.Threading.Tasks;

namespace MrCMS.Services
{
    public static class ExternalLoginServiceExtensions
    {
        public static Task<bool> UserExistsAsync(this IExternalLoginService service, string email)
        {
            return Task.Run(() => service.UserExists(email));
        }
    }
}