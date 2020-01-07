using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Tests.Stubs;

using Xunit;
using MrCMS.Helpers;
using MrCMS.TestSupport;

namespace MrCMS.Tests.Services
{
    public class WidgetServiceTests : MrCMSTest
    {

        [Theory, AutoFakeItEasyData]
        public async Task WidgetService_GetWidget_ReturnsAWidgetWhenIdExists
            ([Frozen] IRepository<Widget> repository, WidgetService sut)
        {

            var textWidget = new BasicMappedWidget();
            A.CallTo(() => repository.Query()).ReturnsAsAsyncQueryable(textWidget);

            var loadedWidget = await sut.GetWidget<BasicMappedWidget>(textWidget.Id);

            loadedWidget.Should().BeSameAs(textWidget);
        }


        [Theory, AutoFakeItEasyData]
        public async Task WidgetService_SaveWidget_ShouldAddWidgetToRepo
            ([Frozen] IRepository<Widget> repository, WidgetService sut)
        {
            var widget = new BasicMappedWidget();

            await sut.SaveWidget(widget);

            A.CallTo(() => repository.Add(widget, default)).MustHaveHappened();
        }

        [Theory, AutoFakeItEasyData]
        public async Task WidgetService_Delete_RemovesWidgetFromRepo
            ([Frozen] IRepository<Widget> repository, WidgetService sut)
        {
            var widget = new BasicMappedWidget();

            await sut.DeleteWidget(widget);

            A.CallTo(() => repository.Delete(widget, default)).MustHaveHappened();
        }
    }
}