using System;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.MessageTemplates;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Web.Apps.Core.Services;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Web.Tests.Apps.Core.Services
{
    public class ResetPasswordServiceTests : InMemoryDatabaseTest
    {
        private readonly SiteSettings _siteSettings;
        private readonly MailSettings _mailSettings;
        private readonly IUserService _userService;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IMessageParser<ResetPasswordMessageTemplate, User> _messageParser;
        private readonly ResetPasswordService _resetPasswordService;

        public ResetPasswordServiceTests()
        {
            _siteSettings = new SiteSettings();
            _mailSettings = new MailSettings();
            _userService = A.Fake<IUserService>();
            _passwordManagementService = A.Fake<IPasswordManagementService>();
            _messageParser = A.Fake<IMessageParser<ResetPasswordMessageTemplate, User>>();
            _resetPasswordService = new ResetPasswordService(_siteSettings, _mailSettings, _userService,
                                                             _passwordManagementService,
                                                             _messageParser);
        }

        [Fact]
        public void ResetPasswordService_SetResetPassword_SetsTheResetPasswordGuid()
        {
            var user = new User();
            A.CallTo(() => _messageParser.GetMessage(user, null, null, null, null, null, null))
             .Returns(new QueuedMessage());

            _resetPasswordService.SetResetPassword(user);

            user.ResetPasswordGuid.Should().HaveValue();
        }

        [Fact]
        public void ResetPasswordService_SetResetPassword_SetsTheResetPasswordExpiry()
        {
            var user = new User();
            A.CallTo(() => _messageParser.GetMessage(user, null, null, null, null, null, null))
             .Returns(new QueuedMessage());

            _resetPasswordService.SetResetPassword(user);

            user.ResetPasswordExpiry.Should().HaveValue();
        }

        [Fact]
        public void ResetPasswordService_SetResetPassword_ShouldSaveAQueuedMessage()
        {
            var user = new User();
            var queuedMessage = new QueuedMessage();
            A.CallTo(() => _messageParser.GetMessage(user, null, null, null, null, null, null))
             .Returns(queuedMessage);

            _resetPasswordService.SetResetPassword(user);

            A.CallTo(() => _messageParser.QueueMessage(queuedMessage, true)).MustHaveHappened();
        }

        [Fact]
        public void ResetPasswordService_ResetPassword_WhenValidCallsSetPasswordOnTheAuthorisationService()
        {
            var guid = Guid.NewGuid();
            var user = new User
            {
                ResetPasswordExpiry = CurrentRequestData.Now.AddDays(1),
                ResetPasswordGuid = guid,
                Email = "test@example.com"
            };
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(user);

            const string password = "password";

            A.CallTo(() => _passwordManagementService.ValidatePassword(password, password)).Returns(true);
            _resetPasswordService.ResetPassword(new ResetPasswordViewModel(guid, user)
            {
                Password = password,
                ConfirmPassword = password,
                Email = "test@example.com"
            });

            A.CallTo(() => _passwordManagementService.SetPassword(user, password, password)).MustHaveHappened();
        }

        [Fact]
        public void ResetPasswordService_ResetPassword_ResetsThePasswordGuid()
        {
            var guid = Guid.NewGuid();
            var user = new User
            {
                ResetPasswordExpiry = CurrentRequestData.Now.AddDays(1),
                ResetPasswordGuid = guid,
                Email = "test@example.com"
            };
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(user);
            const string password = "password";

            A.CallTo(() => _passwordManagementService.ValidatePassword(password, password)).Returns(true);
            _resetPasswordService.ResetPassword(new ResetPasswordViewModel(guid, user)
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
            var guid = Guid.NewGuid();
            var user = new User
            {
                ResetPasswordExpiry = CurrentRequestData.Now.AddDays(1),
                ResetPasswordGuid = guid,
                Email = "test@example.com"
            };
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(user);
            const string password = "password";

            A.CallTo(() => _passwordManagementService.ValidatePassword(password, password)).Returns(true);
            _resetPasswordService.ResetPassword(new ResetPasswordViewModel(guid, user)
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
            var guid = Guid.NewGuid();
            var user = new User
            {
                Email = "test@example.com"
            };

            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(user);
            const string password = "password";
            AssertionExtensions.ShouldThrow<InvalidOperationException>(() =>
                                                                       _resetPasswordService
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