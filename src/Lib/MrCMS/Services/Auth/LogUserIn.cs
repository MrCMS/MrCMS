using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public class LogUserIn : ILogUserIn
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly IGetCurrentUserGuid _getCurrentUserGuid;
        private readonly IEventContext _eventContext;

        public LogUserIn(IAuthorisationService authorisationService, IGetCurrentUserGuid getCurrentUserGuid, IEventContext eventContext)
        {
            _authorisationService = authorisationService;
            _getCurrentUserGuid = getCurrentUserGuid;
            _eventContext = eventContext;
        }

        public async Task Login(User user, bool rememberMe)
        {
            var previousSession = _getCurrentUserGuid.Get();
            await _authorisationService.SetAuthCookie(user, rememberMe);
            _eventContext.Publish<IOnUserLoggedIn, UserLoggedInEventArgs>(new UserLoggedInEventArgs(user, previousSession));
        }
    }
}