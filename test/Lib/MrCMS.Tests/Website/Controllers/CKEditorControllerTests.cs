using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Settings;
using MrCMS.Tests.Website.Controllers.Builders;
using Xunit;

namespace MrCMS.Tests.Website.Controllers
{
    public class CKEditorControllerTests
    {
        [Fact]
        public async Task Config_ReturnsATheSettingsConfigAsAContentResult()
        {
            var siteSettings = new SiteSettings { CKEditorConfig = "test config" };
            var controller = new CKEditorControllerBuilder().WithSettings(siteSettings).Build();

            var result = await controller.Config();

            result.Should().BeOfType<ContentResult>();
            result.Content.Should().Be("test config");
            result.ContentType.Should().Be("application/javascript");
        }
    }
}