using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using Xunit;

namespace MrCMS.Commenting.Tests.Services.CommentingSettingsAdminServiceTests
{
    public class GetAllPageTypes
    {
        [Fact]
        public void ReturnsTypeHelperGetAllConcreteMappedWebpageTypes()
        {
            var expected = TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>();
            var commentingAdminService = new CommentingSettingsAdminServiceBuilder().Build();

            commentingAdminService.GetAllPageTypes().Should().BeEquivalentTo(expected);
        }
    }
}