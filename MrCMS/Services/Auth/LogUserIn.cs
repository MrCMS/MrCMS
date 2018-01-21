using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Website;

namespace MrCMS.Services.Auth
{
    public class LogUserIn : ILogUserIn
    {
        private readonly IAuthorisationService _authorisationService;

        public LogUserIn(IAuthorisationService authorisationService)
        {
            _authorisationService = authorisationService;
        }

        public async Task Login(User user, bool rememberMe)
        {
            var previousSession = CurrentRequestData.UserGuid;
            await _authorisationService.SetAuthCookie(user, rememberMe);
            CurrentRequestData.CurrentUser = user;
            EventContext.Instance.Publish<IOnUserLoggedIn, UserLoggedInEventArgs>(new UserLoggedInEventArgs(user, previousSession));
        }
    }
}