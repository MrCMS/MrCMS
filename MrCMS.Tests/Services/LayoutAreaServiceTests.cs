using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class LayoutAreaServiceTests : InMemoryDatabaseTest
    {
        private readonly LayoutAreaService _layoutAreaService;

        public LayoutAreaServiceTests()
        {
            _layoutAreaService = new LayoutAreaService(Session);
        }

        [Fact]
        public void LayoutAreaService_GetArea_GetsTheAreaByName()
        {
            var layoutAreaService = _layoutAreaService;
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
        public void LayoutAreaService_GetAreaWhereItDoesNotExist_ShouldReturnNull()
        {
            var layoutAreaService = _layoutAreaService;
            var layout = new Layout {Name = "Layout"};
            Session.Transact(session => session.SaveOrUpdate(layout));

            var loadedArea = layoutAreaService.GetArea(layout, "Area.Name");

            loadedArea.Should().BeNull();
        }

        [Fact]
        public void LayoutAreaService_SaveArea_SavesTheArea()
        {
            var layoutArea = new LayoutArea();

            _layoutAreaService.SaveArea(layoutArea);

            Session.QueryOver<LayoutArea>().RowCount().Should().Be(1);
        }

        [Fact]
        public void LayoutAreaService_SaveArea_IfLayoutIsSetAddLayoutAreaToLayoutAreasList()
        {
            var layout = new Layout();
            var layoutArea = new LayoutArea{Layout = layout};

            _layoutAreaService.SaveArea(layoutArea);

            layout.LayoutAreas.Should().Contain(layoutArea);
        }

        [Fact]
        public void LayoutAreaService_GetArea_ShouldReturnNullForInvalidId()
        {
            var layoutAreaService = _layoutAreaService;

            var layoutArea = layoutAreaService.GetArea(-1);

            layoutArea.Should().BeNull();
        }

        [Fact]
        public void LayoutAreaService_GetArea_ShouldReturnLayoutAreaForValidId()
        {
            var layoutArea = new LayoutArea();
            Session.Transact(session => session.SaveOrUpdate(layoutArea));
            var layoutAreaService = _layoutAreaService;

            var loadedLayoutArea = layoutAreaService.GetArea(layoutArea.Id);

            loadedLayoutArea.Should().BeSameAs(layoutArea);
        }

        [Fact]
        public void LayoutAreaService_DeleteArea_DeletesThePassedArea()
        {
            var layoutArea = new LayoutArea();
            Session.Transact(session => session.Save(layoutArea));

            _layoutAreaService.DeleteArea(layoutArea);

            Session.QueryOver<LayoutArea>().RowCount().Should().Be(0);
        }

        [Fact]
        public void LayoutAreaService_DeleteArea_AreaIsRemovedFromLayoutsLayoutAreaList()
        {
            var layout = new Layout();
            var layoutArea = new LayoutArea { Layout = layout };
            layout.LayoutAreas.Add(layoutArea);

            _layoutAreaService.DeleteArea(layoutArea);

            layout.LayoutAreas.Should().NotContain(layoutArea);
        }

        //[Fact]
        //public void LayoutAreaService_SetOrders_ShouldSetOrderToBeTheOrderOfTheWidgetIdInTheArgumentString()
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