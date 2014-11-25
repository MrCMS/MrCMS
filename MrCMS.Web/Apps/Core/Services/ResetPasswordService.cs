using System;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.MessageTemplates;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Services
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly IMessageParser<ResetPasswordMessageTemplate, User> _messageParser;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IUserService _userService;

        public ResetPasswordService(IUserService userService, IPasswordManagementService passwordManagementService,
            IMessageParser<ResetPasswordMessageTemplate, User> messageParser)
        {
            _userService = userService;
            _passwordManagementService = passwordManagementService;
            _messageParser = messageParser;
        }

        public void SetResetPassword(User user)
        {
            user.ResetPasswordExpiry = CurrentRequestData.Now.AddDays(1);
            user.ResetPasswordGuid = Guid.NewGuid();
            _userService.SaveUser(user);

            QueuedMessage queuedMessage = _messageParser.GetMessage(user);
            _messageParser.QueueMessage(queuedMessage);
        }

        public void ResetPassword(ResetPasswordViewModel model)
        {
            User user = _userService.GetUserByEmail(model.Email);

            if (user.ResetPasswordGuid == model.Id && user.ResetPasswordExpiry > CurrentRequestData.Now &&
                _passwordManagementService.ValidatePassword(model.Password, model.ConfirmPassword))
            {
                _passwordManagementService.SetPassword(user, model.Password, model.ConfirmPassword);

                user.ResetPasswordExpiry = null;
                user.ResetPasswordGuid = null;

                _userService.SaveUser(user);
            }
            else
                throw new InvalidOperationException("Unable to reset password, resend forgotten password email");
        }
    }
}