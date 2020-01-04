using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Services.Auth
{
    public class LogFailedLogin : IOnFailedLogin
    {
        private readonly SecuritySettings _securitySettings;
        private readonly IRepository<LoginAttempt> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogFailedLogin(SecuritySettings securitySettings, IRepository<LoginAttempt> repository, IHttpContextAccessor httpContextAccessor)
        {
            _securitySettings = securitySettings;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Execute(UserFailedLoginEventArgs args)
        {
            if (!_securitySettings.LogLoginAttempts)
                return;
            var request = _httpContextAccessor.HttpContext.Request;
            var loginAttempt = new LoginAttempt
            {
                User = args.User,
                Email = args.Email,
                Status = LoginAttemptStatus.Failure,
                IpAddress = request.GetCurrentIP(),
                UserAgent = request.UserAgent()
            };
            await _repository.Add(loginAttempt);
        }
    }
}