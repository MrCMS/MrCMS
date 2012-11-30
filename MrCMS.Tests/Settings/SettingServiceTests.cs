using System;
using FakeItEasy;
using MrCMS.Entities.Settings;
using MrCMS.Settings;
using NHibernate;
using Xunit;
using FluentAssertions;
using MrCMS.Helpers;

namespace MrCMS.Tests.Settings
{
    public class SettingServiceTests : InMemoryDatabaseTest
    {
        [Fact]
        public void SettingService_GetSettingById_CallsSessionGet()
        {
            var session = A.Fake<ISession>();
            var settingService = new SettingService(session);

            settingService.GetSettingById(1);

            A.CallTo(() => session.Get<Setting>(1)).MustHaveHappened();
        }

        [Fact]
        public void SettingService_GetSettingById_ReturnsTheResultOfSessionGet()
        {
            var session = A.Fake<ISession>();
            var setting = new Setting();
            A.CallTo(() => session.Get<Setting>(1)).Returns(setting);
            var settingService = new SettingService(session);

            var settingById = settingService.GetSettingById(1);

            settingById.Should().Be(setting);
        }

        [Fact]
        public void SettingService_DeleteSetting_CallsSessionDelete()
        {
            var session = A.Fake<ISession>();
            var settingService = new SettingService(session);
            var setting = new Setting { Name = "test" };

            settingService.DeleteSetting(setting);

            A.CallTo(() => session.Delete(setting)).MustHaveHappened();
        }

        [Fact]
        public void SettingService_DeleteSetting_NullSettingThrowsArgumentNullException()
        {
            var session = A.Fake<ISession>();
            var settingService = new SettingService(session);

            this.Invoking(tests => settingService.DeleteSetting(null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SettingService_SetSetting_AddsANewSettingToTheSession()
        {
            var settingService = new SettingService(Session);

            settingService.SetSetting("test", "value");

            Session.QueryOver<Setting>().List().Should().HaveCount(1);
        }

        [Fact]
        public void SettingService_SetSettingShouldUpdateExistingSetting()
        {
            var settingService = new SettingService(Session);
            Session.Transact(session => session.Save(new Setting {Name = "test", Value = "value"}));
            settingService.SetSetting("test", "value2");

            var settings = Session.QueryOver<Setting>().List();

            settings.Should().HaveCount(1);
            settings[0].Name.Should().Be("test");
            settings[0].Value.Should().Be("value2");
        }

        [Fact]
        public void SettingService_SetSetting_IfTheKeyIsNullThrowArgumentNullException()
        {
            var settingService = new SettingService(Session);

            this.Invoking(tests => settingService.SetSetting(null, "value")).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SettingService_GetSettingByKey_ReturnsNullIfKeyDoesNotExist()
        {
            var settingService = new SettingService(Session);

            settingService.GetSettingById(-1).Should().BeNull();
        }

        [Fact]
        public void SettingService_GetSettingByKey_ReturnsTheSettingsObjectWithTheValidKey()
        {
            var settingService = new SettingService(Session);
            var setting1 = new Setting {Name = "test", Value = "value"};
            Session.Transact(session => session.Save(setting1));
            var setting2 = new Setting {Name = "test2", Value = "value2"};
            Session.Transact(session => session.Save(setting2));

            settingService.GetSettingByKey("test2").Should().Be(setting2);
        }

        [Fact]
        public void SettingService_GetSettingValueByKey_ReturnsDefaultForNullKey()
        {
            var settingService = new SettingService(Session);

            settingService.GetSettingValueByKey(null, "default").Should().Be("default");
        }

        [Fact]
        public void SettingService_GetSettingValueByKey_ReturnsValueForSetting()
        {
            var settingService = new SettingService(Session);
            var setting1 = new Setting { Name = "test", Value = "value" };
            Session.Transact(session => session.Save(setting1));
            var setting2 = new Setting { Name = "test2", Value = "value2" };
            Session.Transact(session => session.Save(setting2));

            settingService.GetSettingValueByKey("test2", "default").Should().Be("value2");
        }
    }
}