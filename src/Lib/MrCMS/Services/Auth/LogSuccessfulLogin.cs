using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Services.Auth
{
    public class LogSuccessfulLogin : IOnUserLoggedIn
    {
        private readonly ISystemConfigurationProvider _configurationProvider;
        private readonly IRepository<LoginAttempt> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogSuccessfulLogin(ISystemConfigurationProvider configurationProvider, IRepository<LoginAttempt> repository, IHttpContextAccessor httpContextAccessor)
        {
            _configurationProvider = configurationProvider;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Execute(UserLoggedInEventArgs args)
        {
            var securitySettings = await _configurationProvider.GetSystemSettings<SecuritySettings>();
            if (!securitySettings.LogLoginAttempts)
                return;
            var request = _httpContextAccessor.HttpContext.Request;
            var loginAttempt = new LoginAttempt
            {
                User = args.User,
                Email = args.User?.Email,
                Status = LoginAttemptStatus.Success,
                IpAddress = request.GetCurrentIP(),
                UserAgent = request.UserAgent()
            };
            await _repository.Add(loginAttempt);
        }
    }
}