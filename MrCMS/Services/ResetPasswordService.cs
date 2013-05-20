using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Services
{
    public interface IResetPasswordService
    {
        void SetResetPassword(User user);
        void ResetPassword(ResetPasswordViewModel model);
    }

    public class ResetPasswordService : IResetPasswordService
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;
        private readonly IUserService _userService;
        private readonly HttpRequestBase _httpRequest;
        private readonly IAuthorisationService _authorisationService;
        private readonly MailSettings _mailSettings;

        public ResetPasswordService(ISession session, SiteSettings siteSettings, MailSettings mailSettings, IUserService userService, HttpRequestBase httpRequest, IAuthorisationService authorisationService)
        {
            _session = session;
            _siteSettings = siteSettings;
            _mailSettings = mailSettings;
            _userService = userService;
            _httpRequest = httpRequest;
            _authorisationService = authorisationService;
        }

        public void SetResetPassword(User user)
        {
            user.ResetPasswordExpiry = CurrentRequestData.Now.AddDays(1);
            user.ResetPasswordGuid = Guid.NewGuid();
            _userService.SaveUser(user);


            var urlHelper = new UrlHelper(_httpRequest.RequestContext, RouteTable.Routes);
            var resetUrl = _httpRequest.Url.Scheme + "://" + _httpRequest.Url.Authority +
                           urlHelper.Action("PasswordReset", "Login", new { id = user.ResetPasswordGuid });

            var queuedMessage = new QueuedMessage
                                    {
                                        FromAddress = _mailSettings.SystemEmailAddress,
                                        ToAddress = user.Email,
                                        Subject = "Password reset",
                                        Body =
                                            string.Format(
                                                "To reset your password please click <a href=\"{0}\">here</a>", resetUrl),
                                        IsHtml = true
                                    };

            _session.Transact(session => session.SaveOrUpdate(queuedMessage));

            //to do - is this needed with new task system?
            
            TaskExecutor.ExecuteLater(new SendQueuedMessagesTask(_mailSettings, _siteSettings));
        }

        public void ResetPassword(ResetPasswordViewModel model)
        {
            var user = _userService.GetUserByEmail(model.Email);

            if (user.ResetPasswordGuid == model.Id && user.ResetPasswordExpiry > CurrentRequestData.Now)
            {
                _authorisationService.SetPassword(user, model.Password, model.ConfirmPassword);

                user.ResetPasswordExpiry = null;
                user.ResetPasswordGuid = null;

                _userService.SaveUser(user);
            }
            else throw new InvalidOperationException("Unable to reset password, resend forgotten password email");
        }
    }
}