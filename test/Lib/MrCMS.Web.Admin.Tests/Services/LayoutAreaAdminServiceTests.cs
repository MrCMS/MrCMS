using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Caching;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Services
{
    public class LayoutAreaAdminServiceTests
    {
        public LayoutAreaAdminServiceTests()
        {
            _sut = new LayoutAreaAdminService(_layoutAreaRepository,
                _widgetRepository, _mapper, _cacheManager, _widgetLoader);
        }

        private readonly LayoutAreaAdminService _sut;
        private readonly IRepository<LayoutArea> _layoutAreaRepository = A.Fake<IRepository<LayoutArea>>();
        private readonly IRepository<Widget> _widgetRepository = A.Fake<IRepository<Widget>>();


        private readonly IMapper _mapper = A.Fake<IMapper>();
        private readonly ICacheManager _cacheManager = A.Fake<ICacheManager>();
        private IWidgetLoader _widgetLoader = A.Fake<IWidgetLoader>();

        [Fact]
        public async Task LayoutAreaAdminService_DeleteArea_AreaIsRemovedFromLayoutsLayoutAreaList()
        {
            var layout = new Layout();
            var layoutArea = new LayoutArea {Layout = layout};
            layout.LayoutAreas.Add(layoutArea);
            A.CallTo(() => _layoutAreaRepository.Get(123)).Returns(layoutArea);
            await _sut.DeleteArea(123);

            layout.LayoutAreas.Should().NotContain(layoutArea);
        }

        [Fact]
        public async Task LayoutAreaAdminService_DeleteArea_DeletesThePassedArea()
        {
            var layoutArea = new LayoutArea();
            A.CallTo(() => _layoutAreaRepository.Get(123)).Returns(layoutArea);

            await _sut.DeleteArea(123);

            A.CallTo(() => _layoutAreaRepository.Delete(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public async Task LayoutAreaAdminService_GetArea_ShouldReturnLayoutAreaForValidId()
        {
            var layoutArea = A.Dummy<LayoutArea>();
            A.CallTo(() => _layoutAreaRepository.Get(123)).Returns(layoutArea);

            var loadedLayoutArea = await _sut.GetArea(123);

            loadedLayoutArea.Should().BeSameAs(layoutArea);
        }

        [Fact]
        public async Task LayoutAreaAdminService_GetArea_ShouldReturnNullForInvalidId()
        {
            A.CallTo(() => _layoutAreaRepository.Get(-1)).Returns((LayoutArea) null);

            var layoutArea = await _sut.GetArea(-1);

            layoutArea.Should().BeNull();
        }

        [Fact]
        public async Task LayoutAreaAdminService_SaveArea_IfLayoutIsSetAddLayoutAreaToLayoutAreasList()
        {
            var layout = new Layout();
            var layoutArea = new LayoutArea {Layout = layout};
            A.CallTo(() => _layoutAreaRepository.Get(123)).Returns(layoutArea);

            await _sut.Update(new UpdateLayoutAreaModel {Id = 123});

            layout.LayoutAreas.Should().Contain(layoutArea);
        }

        [Fact]
        public async Task LayoutAreaAdminService_SaveArea_SavesTheArea()
        {
            var layoutArea = new LayoutArea();
            A.CallTo(() => _layoutAreaRepository.Get(123)).Returns(layoutArea);

            await _sut.Update(new UpdateLayoutAreaModel {Id = 123});

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