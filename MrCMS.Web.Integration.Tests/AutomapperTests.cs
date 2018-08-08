using System;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Models.WebpageEdit;
using Xunit;

namespace MrCMS.Web.Integration.Tests
{
    public class AutomapperTests
    {
        private const string IntegrationTest = "Integration test";

        //[Fact(Skip = IntegrationTest)]
        [Fact]
        public void IfIdIsNull_ShouldNotMapANewObject()
        {
            var builder = Program.CreateWebHostBuilder(new string[0]);

            var webHost = builder.Build();

            var mapper = webHost.Services.GetRequiredService<IMapper>();

            var layoutTabViewModel = new LayoutTabViewModel { PageTemplateId = null };
            var webpage = new DummyWebpage();

            mapper.Map(layoutTabViewModel, webpage);

            webpage.PageTemplate.Should().BeNull();
        }
        [Fact]
        public void IfIdIsSet_ShouldLoadEntity()
        {
            var builder = Program.CreateWebHostBuilder(new string[0]);

            var webHost = builder.Build();

            var mapper = webHost.Services.GetRequiredService<IMapper>();

            var addWidgetModel = new AddWidgetModel { WebpageId = 3};
            var widget = new DummyWidget();

            mapper.Map(addWidgetModel, widget);

            widget.Webpage.Should().NotBeNull();
        }
        private class DummyWebpage : Webpage { }
        private class DummyWidget : Widget { }
    }
}
