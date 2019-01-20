using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.TestSupport;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class LayoutAreaAdminServiceTests
    {
        public LayoutAreaAdminServiceTests()
        {
            _sut = new LayoutAreaAdminService(_layoutAreaRepository, _webpageRepository,
                _widgetRepository, _pageWidgetSortRepository);
        }

        private readonly LayoutAreaAdminService _sut;
        private readonly IRepository<LayoutArea> _layoutAreaRepository = A.Fake<IRepository<LayoutArea>>();
        private readonly IRepository<Webpage> _webpageRepository = A.Fake<IRepository<Webpage>>();
        private readonly IRepository<Widget> _widgetRepository = A.Fake<IRepository<Widget>>();

        private readonly IRepository<PageWidgetSort> _pageWidgetSortRepository =
            new InMemoryRepository<PageWidgetSort>();

        [Fact]
        public void LayoutAreaAdminService_DeleteArea_AreaIsRemovedFromLayoutsLayoutAreaList()
        {
            var layout = new Layout();
            var layoutArea = new LayoutArea {Layout = layout};
            layout.LayoutAreas.Add(layoutArea);

            _sut.DeleteArea(layoutArea);

            layout.LayoutAreas.Should().NotContain(layoutArea);
        }

        [Fact]
        public void LayoutAreaAdminService_DeleteArea_DeletesThePassedArea()
        {
            var layoutArea = new LayoutArea();

            _sut.DeleteArea(layoutArea);

            A.CallTo(()=>_layoutAreaRepository.Delete(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaAdminService_GetArea_ShouldReturnLayoutAreaForValidId()
        {
            var layoutArea = A.Dummy<LayoutArea>();
            A.CallTo(() => _layoutAreaRepository.Get(123)).Returns(layoutArea);

            var loadedLayoutArea = _sut.GetArea(123);

            loadedLayoutArea.Should().BeSameAs(layoutArea);
        }

        [Fact]
        public void LayoutAreaAdminService_GetArea_ShouldReturnNullForInvalidId()
        {
            A.CallTo(() => _layoutAreaRepository.Get(-1)).Returns(null);

            var layoutArea = _sut.GetArea(-1);

            layoutArea.Should().BeNull();
        }

        [Fact]
        public void LayoutAreaAdminService_SaveArea_IfLayoutIsSetAddLayoutAreaToLayoutAreasList()
        {
            var layout = new Layout();
            var layoutArea = new LayoutArea {Layout = layout};

            _sut.Update(layoutArea);

            layout.LayoutAreas.Should().Contain(layoutArea);
        }

        [Fact]
        public void LayoutAreaAdminService_SaveArea_SavesTheArea()
        {
            var layoutArea = new LayoutArea();

            _sut.Update(layoutArea);

            A.CallTo(() => _layoutAreaRepository.Update(layoutArea)).MustHaveHappened();
        }

        //}
        //    }
        //        widget.DisplayOrder.Should().Be(9 - index);
        //         orders are zero-based
        //        var widget = widgets[index];
        //    {

        //    for (int index = 0; index < widgets.Count; index++)

        //    _layoutAreaService.SetWidgetOrders("10,9,8,7,6,5,4,3,2,1");
        //    widgets.ForEach(widget => Session.Transact(session => session.Save(widget)));
        //    var widgets = Enumerable.Range(1, 10).Select(i => new BasicMappedWidget()).ToList();
        //{
        //public void LayoutAreaAdminService_SetOrders_ShouldSetOrderToBeTheOrderOfTheWidgetIdInTheArgumentString()

        //[Fact]
    }
}