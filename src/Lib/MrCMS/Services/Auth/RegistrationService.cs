using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models.Auth;
using MrCMS.Website;

namespace MrCMS.Services.Auth
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IUserLookup _userLookup;
        private readonly IUserManagementService _userManagementService;
        private readonly IEventContext _eventContext;
        private readonly IGetCurrentUserGuid _getCurrentUserGuid;

        public RegistrationService(IUserLookup userLookup, IPasswordManagementService passwordManagementService,
            IAuthorisationService authorisationService, IUserManagementService userManagementService,
            IEventContext eventContext,
            IGetCurrentUserGuid getCurrentUserGuid)
        {
            _userLookup = userLookup;
            _passwordManagementService = passwordManagementService;
            _authorisationService = authorisationService;
            _userManagementService = userManagementService;
            _eventContext = eventContext;
            _getCurrentUserGuid = getCurrentUserGuid;
        }

        public async Task<User> RegisterUser(RegisterModel model)
        {
            var guid = _getCurrentUserGuid.Get();
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                IsActive = true
            };
            _passwordManagementService.SetPassword(user, model.Password, model.ConfirmPassword);
            await _userManagementService.AddUser(user);
            await _authorisationService.SetAuthCookie(user, false);
            await _eventContext.Publish<IOnUserRegistered, OnUserRegisteredEventArgs>(
                new OnUserRegisteredEventArgs(user, guid));
            return user;
        }

        public bool CheckEmailIsNotRegistered(string email)
        {
            return _userLookup.GetUserByEmail(email) == null;
        }
    }
}