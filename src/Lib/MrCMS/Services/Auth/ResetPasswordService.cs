using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Messages;
using MrCMS.Models.Auth;
using MrCMS.Website;

namespace MrCMS.Services.Auth
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly IUserLookup _userLookup;
        private readonly IEventContext _eventContext;
        private readonly ILogger<ResetPasswordService> _logger;
        private readonly IGetDateTimeNow _getDateTimeNow;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IUserManagementService _userManagementService;

        public ResetPasswordService(IUserManagementService userManagementService, IPasswordManagementService passwordManagementService,
            IUserLookup userLookup, IEventContext eventContext, ILogger<ResetPasswordService> logger,
            IGetDateTimeNow getDateTimeNow)
        {
            _userManagementService = userManagementService;
            _passwordManagementService = passwordManagementService;
            _userLookup = userLookup;
            _eventContext = eventContext;
            _logger = logger;
            _getDateTimeNow = getDateTimeNow;
        }

        public async Task SetResetPassword(User user)
        {
            user.ResetPasswordExpiry = DateTime.UtcNow.AddDays(1);
            user.ResetPasswordGuid = Guid.NewGuid();
            await _userManagementService.SaveUser(user);

            await _eventContext.Publish<IOnUserResetPasswordSet, ResetPasswordEventArgs>(
                new ResetPasswordEventArgs(user));
        }

        public async Task ResetPassword(ResetPasswordViewModel model)
        {
            try
            {

                User user = _userLookup.GetUserByEmail(model.Email);

                if (user.ResetPasswordGuid == model.Id && user.ResetPasswordExpiry > _getDateTimeNow.UtcNow &&
                    _passwordManagementService.ValidatePassword(model.Password, model.ConfirmPassword))
                {
                    _passwordManagementService.SetPassword(user, model.Password, model.ConfirmPassword);

                    user.ResetPasswordExpiry = null;
                    user.ResetPasswordGuid = null;

                    await _userManagementService.SaveUser(user);
                }
                else
                    throw new InvalidOperationException("Unable to reset password, resend forgotten password email");
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                throw;
            }
        }
    }
}