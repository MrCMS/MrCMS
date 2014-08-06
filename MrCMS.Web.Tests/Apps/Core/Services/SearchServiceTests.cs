namespace MrCMS.Web.Tests.Apps.Core.Services
{
    public class SearchServiceTests : InMemoryDatabaseTest
    {
    /*
        [Fact(Skip = "Need to find a way to test lucene indexes")]
        public void SearchService_SearchDocuments_ReturnsAnIEnumerableOfSearchResultModelsWhereTheNameMatches()
        {
            var doc1 = new BasicMappedWebpage { Name = "Test" };
            var doc2 = new BasicMappedWebpage { Name = "Different Name" };
            Session.Transact(session =>
                                 {
                                     session.SaveOrUpdate(doc1);
                                     session.SaveOrUpdate(doc2);
                                 });
            var documentService = GetSearchService();

            var searchResultModels = documentService.Search(new AdminWebpageSearchQuery{Term = "test"});

            searchResultModels.Should().HaveCount(1);
            searchResultModels.First().Name.Should().Be("Test");
        }

        //[Fact(Skip = "Need to find a way to test lucene indexes")]
        //public void SearchService_SearchDocumentsDetailed_ReturnsAnIEnumerableOfSearchResultModelsWhereTheNameMatches()
        //{
        //    var doc1 = new BasicMappedWebpage { Name = "Test" };
        //    var doc2 = new BasicMappedWebpage { Name = "Different Name" };
        //    Session.Transact(session =>
        //                         {
        //                             session.SaveOrUpdate(doc1);
        //                             session.SaveOrUpdate(doc2);
        //                         });
        //    var documentService = GetSearchService();

        //    var searchResultModels = documentService.SearchDocumentsDetailed<BasicMappedWebpage>("Test", null);

        //    searchResultModels.Should().HaveCount(1);
        //    searchResultModels.First().Name.Should().Be("Test");
        //}

        //[Fact(Skip = "Need to find a way to test lucene indexes")]
        //public void SearchService_SearchDocumentsDetailed_FiltersByParentIfIdIsSet()
        //{
        //    var doc1 = new BasicMappedWebpage { Name = "Test" };
        //    var doc2 = new BasicMappedWebpage { Name = "Different Name" };
        //    var doc3 = new BasicMappedWebpage { Name = "Another Name" };
        //    Session.Transact(session =>
        //                         {
        //                             doc1.Parent = doc2;
        //                             session.SaveOrUpdate(doc1);
        //                             session.SaveOrUpdate(doc2);
        //                             session.SaveOrUpdate(doc3);
        //                         });
        //    var documentService = GetSearchService();

        //    var searchResultModels = documentService.SearchDocumentsDetailed<BasicMappedWebpage>("Test", doc3.Id);

        //    searchResultModels.Should().HaveCount(0);
        //}

        //[Fact(Skip = "Need to find a way to test lucene indexes")]
        //public void SearchService_SearchDocumentsDetailed_FiltersByParentIfIdIsSetReturnsIfItIsCorrect()
        //{
        //    var doc1 = new BasicMappedWebpage { Name = "Test" };
        //    var doc2 = new BasicMappedWebpage { Name = "Different Name" };
        //    var doc3 = new BasicMappedWebpage { Name = "Another Name" };
        //    Session.Transact(session =>
        //                         {
        //                             doc1.Parent = doc2;
        //                             session.SaveOrUpdate(doc1);
        //                             session.SaveOrUpdate(doc2);
        //                             session.SaveOrUpdate(doc3);
        //                         });
        //    var documentService = GetSearchService();

        //    var searchResultModels = documentService.SearchDocumentsDetailed<BasicMappedWebpage>("Test", doc2.Id);

        //    searchResultModels.Should().HaveCount(1);
        //    searchResultModels.First().Name.Should().Be("Test");
        //}

        private AdminWebpageSearchService GetSearchService()
        {
            return null;// new SearchService( new CurrentSite(CurrentSite));
        }*/
    }
}