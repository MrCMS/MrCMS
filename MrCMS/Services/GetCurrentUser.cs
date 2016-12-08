using System.Web;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class GetCurrentUser : IGetCurrentUser
    {
        private readonly IUserLookup _userLookup;
        private readonly HttpContextBase _context;

        public GetCurrentUser(HttpContextBase context, IUserLookup userLookup)
        {
            _context = context;
            _userLookup = userLookup;
        }

        public User Get()
        {
            return _userLookup.GetCurrentUser(_context);
        }
    }
}