using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Mapping;
using MrCMS.Web.Admin.Models.WebpageEdit;
using Xunit;

namespace MrCMS.Web.Integration.Tests
{
    public class AutomapperTests
    {
        private const string IntegrationTest = "Integration test - need to work more on setting up database";

        [Fact(Skip = IntegrationTest)]
        public void IfIdIsNull_ShouldNotMapANewObject()
        {
            var builder = Program.CreateWebHostBuilder(new string[0]);

            var webHost = builder.Build();

            var mapper = webHost.Services.GetRequiredService<ISessionAwareMapper>();

            var layoutTabViewModel = new LayoutTabViewModel { PageTemplateId = null };
            var webpage = new DummyWebpage();

            mapper.Map(layoutTabViewModel, webpage);

            webpage.PageTemplate.Should().BeNull();
        }

        private class DummyWebpage : Webpage { }
    }
}
