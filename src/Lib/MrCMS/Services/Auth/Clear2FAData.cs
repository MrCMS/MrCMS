using System.Threading.Tasks;

namespace MrCMS.Services.Auth
{
    public class Clear2FAData : IOnUserLoggedIn
    {
        private readonly IUserManagementService _service;

        public Clear2FAData(IUserManagementService service)
        {
            _service = service;
        }
        public async Task Execute(UserLoggedInEventArgs args)
        {
            var user = args.User;

            user.TwoFactorCode = null;
            user.TwoFactorCodeExpiry = null;

            await _service.SaveUser(user);
        }
    }
}