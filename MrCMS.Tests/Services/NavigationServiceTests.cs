using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class NavigationServiceTests : InMemoryDatabaseTest
    {
        private readonly IDocumentService _documentService;

        public NavigationServiceTests()
        {
            _documentService = A.Fake<IDocumentService>();
        }

        private NavigationService GetDefaultNavigationService()
        {
            return new NavigationService(_documentService, null);
        }

        [Fact]
        public void NavigationService_WebpageTree_WithRootShouldReturnASiteTree()
        {
            var navigationService = GetDefaultNavigationService();
            var websiteTree = navigationService.GetWebsiteTree();
            websiteTree.Should().BeOfType<SiteTree<Webpage>>();
        }

        [Fact]
        public void NavigationService_WebpageTree_SiteTreeShouldRecursivelyReturnNodesToMirrorTheSavedWebpages()
        {
            var page1 = new TextPage { Parent = null };
            var page2 = new TextPage { Parent = page1 };
            var page3 = new TextPage { Parent = page2 };
            var page4 = new TextPage { Parent = page2 };
            page2.Children.Add(page3);
            page2.Children.Add(page4);
            page1.Children.Add(page2);

            Session.Transact(session =>
                                 {
                                     session.SaveOrUpdate(page1);
                                     session.SaveOrUpdate(page2);
                                     session.SaveOrUpdate(page3);
                                     session.SaveOrUpdate(page4);
                                 });

            var navigationService = new NavigationService(new DocumentService(Session, new SiteSettings()), null);
            var websiteTree = navigationService.GetWebsiteTree();

            websiteTree.Children.Should().HaveCount(1);
            websiteTree.Children.First().Children.Should().HaveCount(1);
            websiteTree.Children.First().Children.First().Children.Should().HaveCount(2);
        }

        [Fact]
        public void NavigationService_MediaTree_WithRootShouldReturnASiteTree()
        {
            var navigationService = GetDefaultNavigationService();
            var mediaTree = navigationService.GetMediaTree();
            mediaTree.Should().BeOfType<SiteTree<MediaCategory>>();
        }

        [Fact]
        public void NavigationService_MediaTree_SiteTreeShouldRecursivelyReturnNodesToMirrorTheSavedMediaCategories()
        {
            var category1 = new MediaCategory { Parent = null };
            var category2 = new MediaCategory { Parent = category1 };
            var category3 = new MediaCategory { Parent = category2 };
            var category4 = new MediaCategory { Parent = category2 };
            category2.Children.Add(category3);
            category2.Children.Add(category4);
            category1.Children.Add(category2);

            Session.Transact(session =>
            {
                session.SaveOrUpdate(category1);
                session.SaveOrUpdate(category2);
                session.SaveOrUpdate(category3);
                session.SaveOrUpdate(category4);
            });

            var navigationService = new NavigationService(new DocumentService(Session, new SiteSettings()), null);
            var mediaTree = navigationService.GetMediaTree();

            mediaTree.Children.Should().HaveCount(1);
            mediaTree.Children.First().Children.Should().HaveCount(1);
            mediaTree.Children.First().Children.First().Children.Should().HaveCount(2);
        }

        [Fact]
        public void NavigationService_LayoutList_WithRootShouldReturnASiteTree()
        {
            var navigationService = GetDefaultNavigationService();
            var layoutList = navigationService.GetLayoutList();
            layoutList.Should().BeOfType<SiteTree<Layout>>();
        }

        [Fact]
        public void NavigationService_LayoutList_SiteTreeShouldBeFlatListOfLayouts()
        {
            var layout1 = new Layout { };
            var layout2 = new Layout { };
            var layout3 = new Layout { };
            var layout4 = new Layout { };
            
            Session.Transact(session =>
            {
                session.SaveOrUpdate(layout1);
                session.SaveOrUpdate(layout2);
                session.SaveOrUpdate(layout3);
                session.SaveOrUpdate(layout4);
            });

            var navigationService = new NavigationService(new DocumentService(Session, new SiteSettings()), null);
            var layoutList = navigationService.GetLayoutList();

            layoutList.Children.Should().HaveCount(4);
        }
    }
}