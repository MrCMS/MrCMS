using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class GetCurrentUser : IGetCurrentUser
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserLookup _userLookup;

        public GetCurrentUser(IHttpContextAccessor contextAccessor, IUserLookup userLookup)
        {
            _contextAccessor = contextAccessor;
            _userLookup = userLookup;
        }

        public User Get()
        {
            return _userLookup.GetCurrentUser(_contextAccessor.HttpContext);
        }

        public int? GetId()
        {
            return Get()?.Id;
        }
    }
}