using System.Security.Claims;

namespace MrCMS.Services.Auth
{
    public class LoggedOutEventArgs
    {
        public LoggedOutEventArgs(ClaimsPrincipal user)
        {
            User = user;
        }

        public ClaimsPrincipal User { get; }
    }
}