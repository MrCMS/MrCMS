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
        private readonly IUserLookup _userLookup;
        private readonly IGetNow _getNow;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IUserManagementService _userManagementService;

        public ResetPasswordService(IUserManagementService userManagementService, IPasswordManagementService passwordManagementService,
            IMessageParser<ResetPasswordMessageTemplate, User> messageParser, IUserLookup userLookup,IGetNow getNow)
        {
            _userManagementService = userManagementService;
            _passwordManagementService = passwordManagementService;
            _messageParser = messageParser;
            _userLookup = userLookup;
            _getNow = getNow;
        }

        public void SetResetPassword(User user)
        {
            user.ResetPasswordExpiry = _getNow.Get().AddDays(1);
            user.ResetPasswordGuid = Guid.NewGuid();
            _userManagementService.SaveUser(user);

            QueuedMessage queuedMessage = _messageParser.GetMessage(user);
            _messageParser.QueueMessage(queuedMessage);
        }

        public void ResetPassword(ResetPasswordViewModel model)
        {
            User user = _userLookup.GetUserByEmail(model.Email);

            if (user.ResetPasswordGuid == model.Id && user.ResetPasswordExpiry > _getNow.Get() &&
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