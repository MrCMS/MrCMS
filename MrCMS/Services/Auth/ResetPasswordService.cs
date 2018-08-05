using System;
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
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IUserManagementService _userManagementService;

        public ResetPasswordService(IUserManagementService userManagementService, IPasswordManagementService passwordManagementService,
            IUserLookup userLookup, IEventContext eventContext)
        {
            _userManagementService = userManagementService;
            _passwordManagementService = passwordManagementService;
            _userLookup = userLookup;
            _eventContext = eventContext;
        }

        public void SetResetPassword(User user)
        {
            user.ResetPasswordExpiry = DateTime.UtcNow.AddDays(1);
            user.ResetPasswordGuid = Guid.NewGuid();
            _userManagementService.SaveUser(user);

            _eventContext.Publish<IOnUserResetPasswordSet, ResetPasswordEventArgs>(
                new ResetPasswordEventArgs(user));
        }

        public void ResetPassword(ResetPasswordViewModel model)
        {
            User user = _userLookup.GetUserByEmail(model.Email);

            if (user.ResetPasswordGuid == model.Id && user.ResetPasswordExpiry > DateTime.UtcNow &&
                _passwordManagementService.ValidatePassword(model.Password, model.ConfirmPassword))
            {
                _passwordManagementService.SetPassword(user, model.Password, model.ConfirmPassword);

                user.ResetPasswordExpiry = null;
                user.ResetPasswordGuid = null;

                _userManagementService.SaveUser(user);
            }
            else
                throw new InvalidOperationException("Unable to reset password, resend forgotten password email");
        }
    }
}