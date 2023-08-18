using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class GetCurrentUserGuid : IGetCurrentUserGuid
    {
        public const string UserSessionId = "current.usersessionGuid";

        private readonly IHttpContextAccessor _contextAccessor;

        public GetCurrentUserGuid(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public Task<Guid> Get()
        {
            var context = _contextAccessor.HttpContext;
            // if there's no context, we'll just return a random Guid so that it's not a 'real' session
            if (context == null)
                return Task.FromResult(Guid.NewGuid());

            // next we'll check the claims principal to see if there's a guid there
            var userGuid = context.User.GetUserGuid();
            if (userGuid.HasValue)
                return Task.FromResult(userGuid.Value);

            // otherwise we'll check the cookies as they're not logged in
            var o = context.Request.Cookies[UserSessionId];
            if (o != null && Guid.TryParse(o, out var result))
                return Task.FromResult(result);

            // if there's no cookie, we'll create one and return it
            result = Guid.NewGuid();
            AddCookieToResponse(context, UserSessionId, result.ToString(), DateTime.UtcNow.AddMonths(3));

            return Task.FromResult(result);
        }

        public void ClearCookies()
        {
            var context = _contextAccessor.HttpContext;
            context?.Response.Cookies.Delete(UserSessionId);
        }

        private static void AddCookieToResponse(HttpContext context, string key, string value, DateTime expiry)
        {
            context.Response.Cookies.Append(key, value, new CookieOptions {Expires = expiry});
        }
    }
}