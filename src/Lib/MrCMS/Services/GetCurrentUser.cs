using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class GetCurrentUser : IGetCurrentUser
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserLookup _userLookup;
        private readonly ICacheInHttpContext _cacheInHttpContext;

        public GetCurrentUser(IHttpContextAccessor contextAccessor, IUserLookup userLookup,
            ICacheInHttpContext cacheInHttpContext)
        {
            _contextAccessor = contextAccessor;
            _userLookup = userLookup;
            _cacheInHttpContext = cacheInHttpContext;
        }

        public async Task<User> Get()
        {
            return await _cacheInHttpContext.GetForRequestAsync<User>("current.request.user",
                () => _userLookup.GetCurrentUser(_contextAccessor.HttpContext));
        }
    }
}