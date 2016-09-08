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
    public class LayoutAreaAdminServiceTests : InMemoryDatabaseTest
    {
        public LayoutAreaAdminServiceTests()
        {
            _layoutAreaAdminService = new LayoutAreaAdminService(_layoutAreaRepository, _webpageRepository,
                _widgetRepository, _pageWidgetSortRepository);
        }

        private readonly LayoutAreaAdminService _layoutAreaAdminService;
        private readonly IRepository<LayoutArea> _layoutAreaRepository = new InMemoryRepository<LayoutArea>();
        private readonly IRepository<Webpage> _webpageRepository = new InMemoryRepository<Webpage>();
        private readonly IRepository<Widget> _widgetRepository = new InMemoryRepository<Widget>();

        private readonly IRepository<PageWidgetSort> _pageWidgetSortRepository =
            new InMemoryRepository<PageWidgetSort>();

        [Fact]
        public void LayoutAreaAdminService_DeleteArea_AreaIsRemovedFromLayoutsLayoutAreaList()
        {
            var layout = new Layout();
            var layoutArea = new LayoutArea {Layout = layout};
            layout.LayoutAreas.Add(layoutArea);

            _layoutAreaAdminService.DeleteArea(layoutArea);

            layout.LayoutAreas.Should().NotContain(layoutArea);
        }

        [Fact]
        public void LayoutAreaAdminService_DeleteArea_DeletesThePassedArea()
        {
            var layoutArea = new LayoutArea();
            Session.Transact(session => session.Save(layoutArea));

            _layoutAreaAdminService.DeleteArea(layoutArea);

            Session.QueryOver<LayoutArea>().RowCount().Should().Be(0);
        }

        [Fact]
        public void LayoutAreaAdminService_GetArea_ShouldReturnLayoutAreaForValidId()
        {
            var layoutArea = new LayoutArea();
            Session.Transact(session => session.SaveOrUpdate(layoutArea));
            var layoutAreaService = _layoutAreaAdminService;

            var loadedLayoutArea = layoutAreaService.GetArea(layoutArea.Id);

            loadedLayoutArea.Should().BeSameAs(layoutArea);
        }

        [Fact]
        public void LayoutAreaAdminService_GetArea_ShouldReturnNullForInvalidId()
        {
            var layoutAreaService = _layoutAreaAdminService;

            var layoutArea = layoutAreaService.GetArea(-1);

            layoutArea.Should().BeNull();
        }

        [Fact]
        public void LayoutAreaAdminService_SaveArea_IfLayoutIsSetAddLayoutAreaToLayoutAreasList()
        {
            var layout = new Layout();
            var layoutArea = new LayoutArea {Layout = layout};

            _layoutAreaAdminService.Update(layoutArea);

            layout.LayoutAreas.Should().Contain(layoutArea);
        }

        [Fact]
        public void LayoutAreaAdminService_SaveArea_SavesTheArea()
        {
            var layoutArea = new LayoutArea();

            _layoutAreaAdminService.Update(layoutArea);

            Session.QueryOver<LayoutArea>().RowCount().Should().Be(1);
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