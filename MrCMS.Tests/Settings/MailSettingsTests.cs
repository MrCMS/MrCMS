using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Settings;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Settings
{
    public class MailSettingsTests
    {
        [Fact]
        public void MailSettings_GetTypeName_ReturnsMailSettings()
        {
            var mailSettings = new MailSettings();

            mailSettings.TypeName.Should().Be("Mail Settings");
        }

        [Fact]
        public void MailSettings_GetDivId_ReturnsMailDashSettings()
        {
            var mailSettings = new MailSettings();

            mailSettings.DivId.Should().Be("mail-settings");
        }

        [Fact]
        public void MailSettings_SetViewData_ShouldNotThrow()
        {
            var mailSettings = new MailSettings();

            this.Invoking(tests => mailSettings.SetViewData(A.Fake<ISession>(), A.Fake<ViewDataDictionary>()))
                .ShouldNotThrow();
        }
    }
}