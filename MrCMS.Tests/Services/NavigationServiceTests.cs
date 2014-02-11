using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Indexing.Querying;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using Xunit;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Tests.Services
{
    public class NavigationServiceTests : InMemoryDatabaseTest
    {
        private readonly IDocumentService _documentService;
        private readonly NavigationService _navigationService;

        public NavigationServiceTests()
        {
            _documentService = A.Fake<IDocumentService>();
            var ramDirectory = new RAMDirectory();
            using (new IndexWriter(ramDirectory, new StandardAnalyzer(Version.LUCENE_30),
                                                             IndexWriter.MaxFieldLength.UNLIMITED))
            {

            }

            var indexReader = IndexReader.Open(ramDirectory, true);
            _navigationService = new NavigationService(_documentService, Session, CurrentSite);
            DocumentMetadataHelper.OverrideExistAny = type => false;
        }


        [Fact]
        public void NavigationService_WebpageTree_WithRootShouldReturnASiteTree()
        {
            var websiteTree = _navigationService.GetWebsiteTree();
            websiteTree.Should().BeOfType<SiteTree<Webpage>>();
        }

        [Fact(Skip = "To refactor")]
        public void NavigationService_WebpageTree_SiteTreeShouldRecursivelyReturnNodesToMirrorTheSavedWebpages()
        {
            var page1 = new BasicMappedWebpage
            {
                Parent = null,
                Site = CurrentSite
            };
            var page2 = new BasicMappedWebpage
            {
                Parent = page1,
                Site = CurrentSite
            };
            var page3 = new BasicMappedWebpage
            {
                Parent = page2,
                Site = CurrentSite
            };
            var page4 = new BasicMappedWebpage
            {
                Parent = page2,
                Site = CurrentSite
            };
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

            var navigationService = new NavigationService(new DocumentService(Session, null, new SiteSettings(), CurrentSite), Session, CurrentSite);
            var websiteTree = navigationService.GetWebsiteTree();

            websiteTree.Children.Should().HaveCount(1);
            websiteTree.Children.First().Children.Should().HaveCount(1);
            websiteTree.Children.First().Children.First().Children.Should().HaveCount(2);
        }

        [Fact]
        public void NavigationService_MediaTree_WithRootShouldReturnASiteTree()
        {
            var mediaTree = _navigationService.GetMediaTree();
            mediaTree.Should().BeOfType<SiteTree<MediaCategory>>();
        }

        [Fact(Skip = "To refactor")]
        public void NavigationService_MediaTree_SiteTreeShouldRecursivelyReturnNodesToMirrorTheSavedMediaCategories()
        {
            var category1 = new MediaCategory { Parent = null, Site = CurrentSite };
            var category2 = new MediaCategory { Parent = category1, Site = CurrentSite };
            var category3 = new MediaCategory { Parent = category2, Site = CurrentSite };
            var category4 = new MediaCategory { Parent = category2, Site = CurrentSite };
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

            var navigationService = new NavigationService(new DocumentService(Session, null, new SiteSettings { Site = CurrentSite }, CurrentSite), null, CurrentSite);
            var mediaTree = navigationService.GetMediaTree();

            mediaTree.Children.Should().HaveCount(1);
            mediaTree.Children.First().Children.Should().HaveCount(1);
            mediaTree.Children.First().Children.First().Children.Should().HaveCount(2);
        }

        [Fact]
        public void NavigationService_LayoutList_WithRootShouldReturnASiteTree()
        {
            var layoutList = _navigationService.GetLayoutList();
            layoutList.Should().BeOfType<SiteTree<Layout>>();
        }

        [Fact(Skip = "To refactor")]
        public void NavigationService_LayoutList_SiteTreeShouldBeFlatListOfLayouts()
        {
            var layout1 = new Layout { Site = CurrentSite };
            var layout2 = new Layout { Site = CurrentSite };
            var layout3 = new Layout { Site = CurrentSite };
            var layout4 = new Layout { Site = CurrentSite };

            Session.Transact(session =>
            {
                session.SaveOrUpdate(layout1);
                session.SaveOrUpdate(layout2);
                session.SaveOrUpdate(layout3);
                session.SaveOrUpdate(layout4);
            });

            var navigationService = new NavigationService(
                new DocumentService(Session, null, new SiteSettings(), CurrentSite), Session, CurrentSite);
            var layoutList = navigationService.GetLayoutList();

            layoutList.Children.Should().HaveCount(4);
        }



        [Fact(Skip = "To refactor")]
        public void NavigationService_WebpageTree_WhenShowChildrenInAdminNavIsFalseReturnNoChildren()
        {
            var page1 = new BasicMappedNoChildrenInNavWebpage
            {
                Parent = null,
                Site = CurrentSite
            };
            var page2 = new BasicMappedWebpage
            {
                Parent = page1,
                Site = CurrentSite
            };
            var page3 = new BasicMappedWebpage
            {
                Parent = page1,
                Site = CurrentSite
            };
            var page4 = new BasicMappedWebpage
            {
                Parent = page1,
                Site = CurrentSite
            };
            page1.Children.Add(page3);
            page1.Children.Add(page4);
            page1.Children.Add(page2);

            Session.Transact(session =>
            {
                session.SaveOrUpdate(page1);
                session.SaveOrUpdate(page2);
                session.SaveOrUpdate(page3);
                session.SaveOrUpdate(page4);
            });

            var navigationService = new NavigationService(new DocumentService(Session, null, new SiteSettings(), CurrentSite), Session, CurrentSite);
            var websiteTree = navigationService.GetWebsiteTree();

            websiteTree.Children.Should().HaveCount(1);
            websiteTree.Children.First().Children.Should().HaveCount(0);
        }

        ~NavigationServiceTests()
        {
            DocumentMetadataHelper.OverrideExistAny = null;
        }
    }
}