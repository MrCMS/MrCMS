using System;
using System.Collections.Generic;
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
using NHibernate.Criterion;
using Ninject;

namespace MrCMS.Services
{
    public class UserService : IUserService
    {
        private readonly SiteSettings _siteSettings;
        private readonly IAuthorisationService _authorisationService;
        private readonly ISession _session;

        public UserService(SiteSettings siteSettings, ISession session, IAuthorisationService authorisationService = null)
        {
            _siteSettings = siteSettings ?? MrCMSApplication.Get<SiteSettings>();
            _authorisationService = authorisationService ?? MrCMSApplication.Get<IAuthorisationService>();
            _session = session ?? MrCMSApplication.Get<ISession>();
        }

        public void SaveUser(User user)
        {
            _session.Transact(session => session.SaveOrUpdate(user));
        }

        public User GetUser(int id)
        {
            return _session.Get<User>(id);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _session.QueryOver<User>().Cacheable().List();
        }

        public User GetUserByEmail(string email)
        {
            return
                _session.QueryOver<User>().Where(user => user.Email.IsLike(email, MatchMode.Exact)).Cacheable().
                    SingleOrDefault();
        }

        public User GetUserByResetGuid(Guid resetGuid)
        {
            return
                _session.QueryOver<User>()
                    .Where(user => user.ResetPasswordGuid == resetGuid && user.ResetPasswordExpiry >= DateTime.UtcNow)
                    .Cacheable().SingleOrDefault();
        }

        public User GetCurrentUser(HttpContextBase context)
        {
            return context.User != null ? GetUserByEmail(context.User.Identity.Name) : null;
        }

        public User GetCurrentUser(HttpContext context)
        {
            return context.User != null ? GetUserByEmail(context.User.Identity.Name) : null;
        }

        public void SetPassword(int userId, string password)
        {
            var user = GetUser(userId);

            _authorisationService.SetPassword(user, password, password);

            SaveUser(user);
        }

        public void SaveRole(UserRole role)
        {
            _session.Transact(session => session.SaveOrUpdate(role));
        }

        public UserRole GetRole(int id)
        {
            return _session.Get<UserRole>(id);
        }

        public IEnumerable<UserRole> GetAllRoles()
        {
            return _session.QueryOver<UserRole>().Cacheable().List();
        }

        public UserRole GetRoleByName(string name)
        {
            return
                _session.QueryOver<UserRole>().Where(role => role.Name.IsLike(name, MatchMode.Exact)).Cacheable().
                    SingleOrDefault();
        }

        public void DeleteRole(UserRole role)
        {
            _session.Transact(session => session.Delete(role));
        }

        public void SetResetPassword(User user)
        {
            user.ResetPasswordExpiry = DateTime.UtcNow.AddDays(1);
            user.ResetPasswordGuid = Guid.NewGuid();
            SaveUser(user);

            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext, RouteTable.Routes);
            var resetUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                           urlHelper.Action("PasswordReset", "Login", new { id = user.ResetPasswordGuid });

            var queuedMessage = new QueuedMessage
                                    {
                                        FromAddress = _siteSettings.SystemEmailAddress,
                                        ToAddress = user.Email,
                                        Subject = _siteSettings.SiteName + " Password reset",
                                        Body =
                                            string.Format(
                                                "To reset your password please click <a href=\"{0}\">here</a>", resetUrl),
                                        IsHtml = true
                                    };

            _session.Transact(session => session.SaveOrUpdate(queuedMessage));

            TaskExecutor.ExecuteLater(new SendQueuedMessagesTask());
        }

        public void ResetPassword(ResetPasswordViewModel model)
        {
            var user = GetUserByEmail(model.Email);

            if (user.ResetPasswordGuid == model.Id && user.ResetPasswordExpiry > DateTime.UtcNow)
            {
                _authorisationService.SetPassword(user, model.Password, model.ConfirmPassword);

                user.ResetPasswordExpiry = null;
                user.ResetPasswordGuid = null;

                SaveUser(user);
            }
            else throw new InvalidOperationException("Unable to reset password, resend forgotten password email");
        }

    }
}