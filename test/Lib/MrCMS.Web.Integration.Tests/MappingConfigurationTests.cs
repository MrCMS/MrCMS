using System;
using FakeItEasy;
using FluentAssertions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Web;
using MrCMS.Mapping;
using MrCMS.Web.Admin;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Models.WebpageEdit;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Apps.Articles;
using MrCMS.Web.Apps.Core;
using MrCMS.Web.Apps.Core.Pages;
using NHibernate;
using Xunit;

namespace MrCMS.Web.Integration.Tests
{
    public class MappingConfigurationTests
    {
        private static TypeAdapterConfig CreateConfig()
        {
            var appContext = new MrCMSAppContext();
            appContext.RegisterApp<MrCMSAdmin>();
            appContext.RegisterApp<MrCMSCoreApp>();
            appContext.RegisterApp<MrCMSArticlesApp>();
            return MrCMSMappingConfiguration.Create(appContext, typeof(Program).Assembly);
        }

        private static ISessionAwareMapper CreateMapper()
        {
            var services = new ServiceCollection();
            services.AddSingleton(A.Fake<IWebpageRolesAdminService>());
            return new SessionAwareMapper(CreateConfig(), A.Fake<ISession>(), services.BuildServiceProvider());
        }

        [Fact]
        public void AllRegisteredMappingsCompile()
        {
            var config = CreateConfig();

            foreach (var rule in config.RuleMap.Keys)
            {
                // abstract destinations (e.g. Widget, Webpage) can only be mapped onto an existing instance
                if (!rule.Destination.IsAbstract)
                    config.CreateMapExpression(rule, MapType.Map);
                config.CreateMapExpression(rule, MapType.MapToTarget);
            }
        }

        [Fact]
        public void NullLookupId_ShouldNotCreateANewEntity()
        {
            var webpage = new TextPage();

            CreateMapper().Map(new LayoutTabViewModel { PageTemplateId = null }, (Webpage)webpage);

            webpage.PageTemplate.Should().BeNull();
        }

        [Fact]
        public void BaseTypeMapping_AppliesToWebpageSubtypes()
        {
            var publishOn = new DateTime(2026, 1, 2);
            Webpage webpage = new TextPage();

            CreateMapper().Map(new UpdateWebpageViewModel { PublishOn = publishOn }, webpage);

            webpage.PublishOn.Should().Be(publishOn);
        }

        [Fact]
        public void Permissions_ChangingPasswordIssuesNewAccessToken()
        {
            var originalToken = Guid.NewGuid();
            var webpage = new TextPage { Password = "old", PasswordAccessToken = originalToken };

            CreateMapper().Map(new PermissionsTabViewModel
            {
                HasCustomPermissions = true,
                PermissionType = WebpagePermissionType.PasswordBased,
                Password = " new "
            }, (Webpage)webpage);

            webpage.Password.Should().Be("new");
            webpage.PasswordAccessToken.Should().NotBeNull().And.NotBe(originalToken);
        }

        [Fact]
        public void Permissions_SamePasswordKeepsAccessToken()
        {
            var originalToken = Guid.NewGuid();
            var webpage = new TextPage { Password = "same", PasswordAccessToken = originalToken };

            CreateMapper().Map(new PermissionsTabViewModel
            {
                HasCustomPermissions = true,
                PermissionType = WebpagePermissionType.PasswordBased,
                Password = "same"
            }, (Webpage)webpage);

            webpage.PasswordAccessToken.Should().Be(originalToken);
        }

        [Fact]
        public void Permissions_NotPasswordBasedClearsPassword()
        {
            var webpage = new TextPage { Password = "old", PasswordAccessToken = Guid.NewGuid() };

            CreateMapper().Map(new PermissionsTabViewModel { HasCustomPermissions = false }, (Webpage)webpage);

            webpage.Password.Should().BeNull();
            webpage.PasswordAccessToken.Should().BeNull();
        }
    }
}
