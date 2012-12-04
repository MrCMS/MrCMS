using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class LayoutAreaServiceTests : InMemoryDatabaseTest
    {
        [Fact]
        public void LayoutAreaService_GetArea_GetsTheAreaByName()
        {
            var layoutAreaService = new LayoutAreaService(Session);
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
            var layoutAreaService = new LayoutAreaService(Session);
            var layout = new Layout {Name = "Layout"};
            Session.Transact(session => session.SaveOrUpdate(layout));

            var loadedArea = layoutAreaService.GetArea(layout, "Area.Name");

            loadedArea.Should().BeNull();
        }

        [Fact]
        public void LayoutAreaService_SaveArea_CallsSessionSaveOnTheArea()
        {
            var layoutArea = new LayoutArea();
            var session = A.Fake<ISession>();
            var layoutAreaService = new LayoutAreaService(session);

            layoutAreaService.SaveArea(layoutArea);

            A.CallTo(() => session.SaveOrUpdate(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaService_SaveArea_IfLayoutIsSetAddLayoutAreaToLayoutAreasList()
        {
            var layout = new Layout {LayoutAreas = new List<LayoutArea>()};
            var layoutArea = new LayoutArea{Layout = layout};
            var session = A.Fake<ISession>();
            var layoutAreaService = new LayoutAreaService(session);

            layoutAreaService.SaveArea(layoutArea);

            layout.LayoutAreas.Should().Contain(layoutArea);
        }

        [Fact]
        public void LayoutAreaService_GetArea_ShouldReturnNullForInvalidId()
        {
            var layoutAreaService = new LayoutAreaService(Session);

            var layoutArea = layoutAreaService.GetArea(-1);

            layoutArea.Should().BeNull();
        }

        [Fact]
        public void LayoutAreaService_GetArea_ShouldReturnLayoutAreaForValidId()
        {
            var layoutArea = new LayoutArea();
            Session.Transact(session => session.SaveOrUpdate(layoutArea));
            var layoutAreaService = new LayoutAreaService(Session);

            var loadedLayoutArea = layoutAreaService.GetArea(layoutArea.Id);

            loadedLayoutArea.Should().BeSameAs(layoutArea);
        }

        [Fact]
        public void LayoutAreaService_DeleteArea_CallsSessionDeleteOnThePassedArea()
        {
            var session = A.Fake<ISession>();
            var layoutAreaService = new LayoutAreaService(session);

            var layoutArea = new LayoutArea();
            layoutAreaService.DeleteArea(layoutArea);

            A.CallTo(() => session.Delete(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaService_DeleteArea_AreaIsRemovedFromLayoutsLayoutAreaList()
        {
            var layout = new Layout { LayoutAreas = new List<LayoutArea>() };
            var layoutArea = new LayoutArea { Layout = layout };
            layout.LayoutAreas.Add(layoutArea);
            var layoutAreaService = new LayoutAreaService(Session);

            layoutAreaService.DeleteArea(layoutArea);

            layout.LayoutAreas.Should().NotContain(layoutArea);
        }
    }
}