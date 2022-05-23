using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MrCMS.Services
{
    public class GetCurrentUserGuid : IGetCurrentUserGuid
    {
        public const string UserSessionId = "current.usersessionGuid";

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGetCurrentUser _getCurrentUser;

        public GetCurrentUserGuid(IHttpContextAccessor contextAccessor, IGetCurrentUser getCurrentUser)
        {
            _contextAccessor = contextAccessor;
            _getCurrentUser = getCurrentUser;
        }

        public async Task<Guid> Get()
        {
            var user = await _getCurrentUser.Get();
            if (user != null)
                return user.Guid;

            var context = _contextAccessor.HttpContext;
            // if there's no context, we'll just return a random Guid so that it's not a 'real' session
            if (context == null)
                return Guid.NewGuid();
            
            var o = context.Request.Cookies[UserSessionId];
            if (o != null && Guid.TryParse(o, out var result)) 
                return result;
            
            result = Guid.NewGuid();
            AddCookieToResponse(context, UserSessionId, result.ToString(), DateTime.UtcNow.AddMonths(3));

            return result;
        }

        public void ClearCookies()
        {
            var context = _contextAccessor.HttpContext;
            context?.Response.Cookies.Delete(UserSessionId);
        }

        private static void AddCookieToResponse(HttpContext context, string key, string value, DateTime expiry)
        {
            context.Response.Cookies.Append(key, value, new CookieOptions { Expires = expiry });
        }
    }
}