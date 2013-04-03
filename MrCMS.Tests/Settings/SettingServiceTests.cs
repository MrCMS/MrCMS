using System;
using FakeItEasy;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Settings;
using MrCMS.Website;
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
            var settingService = GetSettingService(session);

            settingService.GetSettingById(1);

            A.CallTo(() => session.Get<Setting>(1)).MustHaveHappened();
        }

        [Fact]
        public void SettingService_GetSettingById_ReturnsTheResultOfSessionGet()
        {
            var session = A.Fake<ISession>();
            var setting = new Setting();
            A.CallTo(() => session.Get<Setting>(1)).Returns(setting);
            var settingService = GetSettingService(session);

            var settingById = settingService.GetSettingById(1);

            settingById.Should().Be(setting);
        }

        [Fact]
        public void SettingService_DeleteSetting_CallsSessionDelete()
        {
            var session = A.Fake<ISession>();
            var settingService = GetSettingService(session);
            var setting = new Setting { Name = "test" };

            settingService.DeleteSetting(setting);

            A.CallTo(() => session.Delete(setting)).MustHaveHappened();
        }

        [Fact]
        public void SettingService_DeleteSetting_NullSettingThrowsArgumentNullException()
        {
            var session = A.Fake<ISession>();
            var settingService = GetSettingService(session);

            this.Invoking(tests => settingService.DeleteSetting(null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SettingService_SetSetting_AddsANewSettingToTheSession()
        {
            var settingService = GetSettingService();

            var site = new Site();
            Session.Transact(session => session.Save(site));

            settingService.SetSetting(site, "test", "value");

            Session.QueryOver<Setting>().List().Should().HaveCount(1);
        }

        [Fact]
        public void SettingService_SetSettingShouldUpdateExistingSetting()
        {
            var settingService = GetSettingService();
            var site = new Site();
            CurrentRequestData.CurrentSite = site;
            Session.Transact(session => session.Save(site));
            Session.Transact(session => session.Save(new Setting {Name = "test", Value = "value", Site = site}));
            settingService.SetSetting(site, "test", "value2");

            var settings = Session.QueryOver<Setting>().List();

            settings.Should().HaveCount(1);
            settings[0].Name.Should().Be("test");
            settings[0].Value.Should().Be("value2");
        }

        [Fact]
        public void SettingService_SetSetting_IfTheKeyIsNullThrowArgumentNullException()
        {
            var site = new Site();
            var settingService = GetSettingService();

            this.Invoking(tests => settingService.SetSetting(site, null, "value")).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SettingService_GetSettingByKey_ReturnsNullIfKeyDoesNotExist()
        {
            var settingService = GetSettingService();

            settingService.GetSettingById(-1).Should().BeNull();
        }

        [Fact]
        public void SettingService_GetSettingByKey_ReturnsTheSettingsObjectWithTheValidKey()
        {
            var settingService = GetSettingService();
            var setting1 = new Setting { Name = "test", Value = "value", Site = CurrentSite };
            Session.Transact(session => session.Save(setting1));
            var setting2 = new Setting { Name = "test2", Value = "value2", Site = CurrentSite };
            Session.Transact(session => session.Save(setting2));

            settingService.GetSettingByKey(CurrentSite, "test2").Should().Be(setting2);
        }

        [Fact]
        public void SettingService_GetSettingByKey_ReturnsNullIfKeyIsNull()
        {
            var site = new Site();
            var settingService = GetSettingService();

            settingService.GetSettingByKey(site, null).Should().Be(null);
        }

        [Fact]
        public void SettingService_GetSettingByKey_ReturnsNullIfTheKeyDoesNotExist()
        {
            var settingService = GetSettingService();
            var site = new Site();
            var setting1 = new Setting { Name = "test", Value = "value", Site = site };
            Session.Transact(session => session.Save(setting1));

            settingService.GetSettingByKey(site, "test2").Should().Be(null);
        }

        [Fact]
        public void SettingService_GetSettingValueByKey_ReturnsDefaultForNullKey()
        {
            var site = new Site();
            var settingService = GetSettingService();

            settingService.GetSettingValueByKey(site, null, "default").Should().Be("default");
        }

        [Fact]
        public void SettingService_GetSettingValueByKey_ReturnsValueForSetting()
        {
            var settingService = GetSettingService();
            var setting1 = new Setting { Name = "test", Value = "value", Site = CurrentSite };
            Session.Transact(session => session.Save(setting1));
            var setting2 = new Setting { Name = "test2", Value = "value2", Site = CurrentSite };
            Session.Transact(session => session.Save(setting2));

            settingService.GetSettingValueByKey(CurrentSite, "test2", "default").Should().Be("value2");
        }

        [Fact]
        public void SettingService_GetSettingValueByKey_DefaultWhenKeyDoesNotExist()
        {
            var settingService = GetSettingService();
            var site = new Site();
            var setting1 = new Setting { Name = "test", Value = "value", Site = site };
            Session.Transact(session => session.Save(setting1));

            settingService.GetSettingValueByKey(site, "test2", "default").Should().Be("default");
        }

        private static SettingService GetSettingService(ISession session = null)
        {
            var settingService = new SettingService(session ?? Session);
            return settingService;
        }
    }
}