using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Services;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class LayoutAreaAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly LayoutAreaAdminService _layoutAreaAdminService;

        public LayoutAreaAdminServiceTests(IDocumentService documentService)
        {
            _layoutAreaAdminService = new LayoutAreaAdminService(Session, documentService);
        }

        [Fact]
        public void LayoutAreaAdminService_GetArea_GetsTheAreaByName()
        {
            var layoutAreaService = _layoutAreaAdminService;
            var layout = new Layout {Name = "Layout"};
            var layoutArea = new LayoutArea {Layout = layout, AreaName = "Area.Name"};
            Session.Transact(session =>
                                 {
                                     session.SaveOrUpdate(layout);
                                     session.SaveOrUpdate(layoutArea);
                                 });

            var loadedArea = layoutAreaService.GetArea(layout, "Area.Name");

            loadedArea.Should().BeSameAs(layoutArea);
        }

        [Fact]
        public void LayoutAreaAdminService_GetAreaWhereItDoesNotExist_ShouldReturnNull()
        {
            var layoutAreaService = _layoutAreaAdminService;
            var layout = new Layout {Name = "Layout"};
            Session.Transact(session => session.SaveOrUpdate(layout));

            var loadedArea = layoutAreaService.GetArea(layout, "Area.Name");

            loadedArea.Should().BeNull();
        }

        [Fact]
        public void LayoutAreaAdminService_SaveArea_SavesTheArea()
        {
            var layoutArea = new LayoutArea();

            _layoutAreaAdminService.SaveArea(layoutArea);

            Session.QueryOver<LayoutArea>().RowCount().Should().Be(1);
        }

        [Fact]
        public void LayoutAreaAdminService_SaveArea_IfLayoutIsSetAddLayoutAreaToLayoutAreasList()
        {
            var layout = new Layout();
            var layoutArea = new LayoutArea{Layout = layout};

            _layoutAreaAdminService.SaveArea(layoutArea);

            layout.LayoutAreas.Should().Contain(layoutArea);
        }

        [Fact]
        public void LayoutAreaAdminService_GetArea_ShouldReturnNullForInvalidId()
        {
            var layoutAreaService = _layoutAreaAdminService;

            var layoutArea = layoutAreaService.GetArea(-1);

            layoutArea.Should().BeNull();
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
        public void LayoutAreaAdminService_DeleteArea_DeletesThePassedArea()
        {
            var layoutArea = new LayoutArea();
            Session.Transact(session => session.Save(layoutArea));

            _layoutAreaAdminService.DeleteArea(layoutArea);

            Session.QueryOver<LayoutArea>().RowCount().Should().Be(0);
        }

        [Fact]
        public void LayoutAreaAdminService_DeleteArea_AreaIsRemovedFromLayoutsLayoutAreaList()
        {
            var layout = new Layout();
            var layoutArea = new LayoutArea { Layout = layout };
            layout.LayoutAreas.Add(layoutArea);

            _layoutAreaAdminService.DeleteArea(layoutArea);

            layout.LayoutAreas.Should().NotContain(layoutArea);
        }

        //[Fact]
        //public void LayoutAreaAdminService_SetOrders_ShouldSetOrderToBeTheOrderOfTheWidgetIdInTheArgumentString()
        //{
        //    var widgets = Enumerable.Range(1, 10).Select(i => new BasicMappedWidget()).ToList();
        //    widgets.ForEach(widget => Session.Transact(session => session.Save(widget)));

        //    _layoutAreaService.SetWidgetOrders("10,9,8,7,6,5,4,3,2,1");

        //    for (int index = 0; index < widgets.Count; index++)
        //    {
        //        var widget = widgets[index];
        //         orders are zero-based
        //        widget.DisplayOrder.Should().Be(9 - index);
        //    }
        //}
    }
}