using System;
using Microsoft.AspNetCore.Http;

namespace MrCMS.Services
{
    public class GetCurrentUserGuid : IGetCurrentUserGuid
    {
        private const string UserSessionId = "current.usersessionGuid";

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGetCurrentUser _getCurrentUser;

        public GetCurrentUserGuid(IHttpContextAccessor contextAccessor, IGetCurrentUser getCurrentUser)
        {
            _contextAccessor = contextAccessor;
            _getCurrentUser = getCurrentUser;
        }

        public Guid Get()
        {
            var user = _getCurrentUser.Get();
            if (user != null)
                return user.Guid;

            var context = _contextAccessor.HttpContext;
            string o = context.Request.Cookies[UserSessionId];
            Guid result;
            if (o == null || !Guid.TryParse(o, out result))
            {
                result = Guid.NewGuid();
                AddCookieToResponse(context, UserSessionId, result.ToString(), DateTime.UtcNow.AddMonths(3));
            }

            return result;
        }

        private static void AddCookieToResponse(HttpContext context, string key, string value, DateTime expiry)
        {
            context.Response.Cookies.Append(key,value,new CookieOptions{Expires = expiry});
        }

    }
}