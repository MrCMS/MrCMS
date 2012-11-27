using System;
using System.Web;
using FakeItEasy;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace MrCMS.Tests.Services
{
    public class ResetPasswordServiceTests : InMemoryDatabaseTest
    {
        [Fact]
        public void ResetPasswordService_SetResetPassword_SetsTheResetPasswordGuid()
        {
            var httpRequestBase = A.Fake<HttpRequestBase>();
            A.CallTo(() => httpRequestBase.Url).Returns(new Uri("http://www.example.com"));
            var resetPasswordService = new ResetPasswordService(Session, new SiteSettings(), new MailSettings(),
                                                                A.Fake<UserService>(), httpRequestBase,
                                                                A.Fake<IAuthorisationService>());

            var user = new User();

            resetPasswordService.SetResetPassword(user);

            user.ResetPasswordGuid.Should().HaveValue();
        }

        [Fact]
        public void ResetPasswordService_SetResetPassword_SetsTheResetPasswordExpiry()
        {
            var httpRequestBase = A.Fake<HttpRequestBase>();
            A.CallTo(() => httpRequestBase.Url).Returns(new Uri("http://www.example.com"));
            var resetPasswordService = new ResetPasswordService(Session, new SiteSettings(), new MailSettings(),
                                                                A.Fake<UserService>(), httpRequestBase,
                                                                A.Fake<IAuthorisationService>());

            var user = new User();

            resetPasswordService.SetResetPassword(user);

            user.ResetPasswordExpiry.Should().HaveValue();
        }

        [Fact]
        public void ResetPasswordService_SetResetPassword_ShouldSaveAQueuedMessage()
        {
            var httpRequestBase = A.Fake<HttpRequestBase>();
            A.CallTo(() => httpRequestBase.Url).Returns(new Uri("http://www.example.com"));
            var resetPasswordService = new ResetPasswordService(Session, new SiteSettings(), new MailSettings(),
                                                                A.Fake<UserService>(), httpRequestBase,
                                                                A.Fake<IAuthorisationService>());

            var user = new User();

            resetPasswordService.SetResetPassword(user);
            Session.QueryOver<QueuedMessage>().List().Should().HaveCount(1);
        }

        [Fact]
        public void ResetPasswordService_SetResetPassword_ShouldQueueASendMessagesTask()
        {
            var httpRequestBase = A.Fake<HttpRequestBase>();
            A.CallTo(() => httpRequestBase.Url).Returns(new Uri("http://www.example.com"));
            var resetPasswordService = new ResetPasswordService(Session, new SiteSettings(), new MailSettings(),
                                                                A.Fake<UserService>(), httpRequestBase,
                                                                A.Fake<IAuthorisationService>());

            var user = new User();

            resetPasswordService.SetResetPassword(user);

            TaskExecutor.TasksToExecute.Value.OfType<SendQueuedMessagesTask>().Should().HaveCount(1);
        }

        [Fact]
        public void ResetPasswordService_ResetPassword_CallsSetPasswordOnTheAuthorisationService()
        {
            var authorisationService = A.Fake<IAuthorisationService>();
            var userService = A.Fake<IUserService>();
            var resetPasswordService = new ResetPasswordService(Session, new SiteSettings(), new MailSettings(),
                                                                userService, A.Fake<HttpRequestBase>(),
                                                                authorisationService);

            var guid = Guid.NewGuid();
            var user = new User
            {
                ResetPasswordExpiry = DateTime.UtcNow.AddDays(1),
                ResetPasswordGuid = guid,
                Email = "test@example.com"
            };

            A.CallTo(() => userService.GetUserByEmail("test@example.com")).Returns(user);

            const string password = "password";
            resetPasswordService.ResetPassword(new ResetPasswordViewModel(guid, user)
            {
                Password = password,
                ConfirmPassword = password,
                Email = "test@example.com"
            });

            A.CallTo(() => authorisationService.SetPassword(user, password, password)).MustHaveHappened();
        }

        [Fact]
        public void ResetPasswordService_ResetPassword_ResetsThePasswordGuid()
        {
            var authorisationService = A.Fake<IAuthorisationService>();
            var userService = A.Fake<IUserService>();
            var resetPasswordService = new ResetPasswordService(Session, new SiteSettings(), new MailSettings(),
                                                                userService, A.Fake<HttpRequestBase>(),
                                                                authorisationService);

            var guid = Guid.NewGuid();
            var user = new User
            {
                ResetPasswordExpiry = DateTime.UtcNow.AddDays(1),
                ResetPasswordGuid = guid,
                Email = "test@example.com"
            };

            A.CallTo(() => userService.GetUserByEmail("test@example.com")).Returns(user);

            const string password = "password";
            resetPasswordService.ResetPassword(new ResetPasswordViewModel(guid, user)
            {
                Password = password,
                ConfirmPassword = password,
                Email = "test@example.com"
            });

            user.ResetPasswordGuid.Should().NotHaveValue();
        }

        [Fact]
        public void ResetPasswordService_ResetPassword_ResetsThePasswordExpiry()
        {
            var authorisationService = A.Fake<IAuthorisationService>();
            var userService = A.Fake<IUserService>();
            var resetPasswordService = new ResetPasswordService(Session, new SiteSettings(), new MailSettings(),
                                                                userService, A.Fake<HttpRequestBase>(),
                                                                authorisationService);

            var guid = Guid.NewGuid();
            var user = new User
            {
                ResetPasswordExpiry = DateTime.UtcNow.AddDays(1),
                ResetPasswordGuid = guid,
                Email = "test@example.com"
            };

            A.CallTo(() => userService.GetUserByEmail("test@example.com")).Returns(user);

            const string password = "password";
            resetPasswordService.ResetPassword(new ResetPasswordViewModel(guid, user)
            {
                Password = password,
                ConfirmPassword = password,
                Email = "test@example.com"
            });

            user.ResetPasswordExpiry.Should().NotHaveValue();
        }

        [Fact]
        public void ResetPasswordService_ResetPassword_ThrowsAnExceptionIfTheGuidIsNotCorrect()
        {
            var authorisationService = A.Fake<IAuthorisationService>();
            var userService = A.Fake<IUserService>();
            var resetPasswordService = new ResetPasswordService(Session, new SiteSettings(), new MailSettings(),
                                                                userService, A.Fake<HttpRequestBase>(),
                                                                authorisationService);

            var guid = Guid.NewGuid();
            var user = new User
            {
                Email = "test@example.com"
            };

            A.CallTo(() => userService.GetUserByEmail("test@example.com")).Returns(user);

            const string password = "password";
            AssertionExtensions.ShouldThrow<InvalidOperationException>(() =>
                                                                       resetPasswordService
                                                                           .ResetPassword(
                                                                               new ResetPasswordViewModel
                                                                                   (guid, user)
                                                                                   {
                                                                                       Password = password,
                                                                                       ConfirmPassword = password,
                                                                                       Email = "test@example.com"
                                                                                   }));
        }
    }
}