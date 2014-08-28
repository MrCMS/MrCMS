using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Tests.Website.Controllers.Builders;
using Xunit;

namespace MrCMS.Tests.Website.Controllers
{
    public class WebpageControllerTests
    {
        [Fact]
        public void Show_ReturnsThePassedWebpageAsAViewResult()
        {
            var controller = new WebpageControllerBuilder().Build();
            var webpage = A.Dummy<Webpage>();

            var result = controller.Show(webpage);

            result.Should().BeOfType<ViewResult>();
            result.Model.Should().Be(webpage);
        }
    }
}