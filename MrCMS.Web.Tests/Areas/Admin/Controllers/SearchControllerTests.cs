using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class SearchControllerTests
    {
        private static IDocumentService documentService;
        private static INavigationService navigationService;

        [Fact]
        public void SearchController_GetSearchResults_NullStringShouldReturnEmptyObject()
        {
            var searchController = GetSearchController();

            var result = searchController.GetSearchResults(null, null);

            result.Data.Should().BeOfType<object>();
        }

        private static SearchController GetSearchController()
        {
            documentService = A.Fake<IDocumentService>();
            navigationService = A.Fake<INavigationService>();
            var searchController = new SearchController(documentService, navigationService);
            return searchController;
        }

        [Fact]
        public void SearchController_GetSearchResults_EmptyStringShouldReturnEmptyObject()
        {
            var searchController = GetSearchController();

            var result = searchController.GetSearchResults("", null);

            result.Data.Should().BeOfType<object>();
        }
        [Fact]
        public void SearchController_GetSearchResults_WhiteSpaceStringgShouldReturnEmptyObject()
        {
            var searchController = GetSearchController();

            var result = searchController.GetSearchResults("  ", null);

            result.Data.Should().BeOfType<object>();
        }

        [Fact]
        public void SearchController_GetSearchResults_CallsDocumentServiceSearchDocuments()
        {
            var searchController = GetSearchController();

            searchController.GetSearchResults("test", null);

            A.CallTo(() => documentService.SearchDocuments<Document>("test")).MustHaveHappened();
        }

        [Fact]
        public void SearchController_GetSearchResults_ReturnsIEnumerableSearchResultModels()
        {
            var searchController = GetSearchController();

            IEnumerable<SearchResultModel> searchResultModels = A.CollectionOfFake<SearchResultModel>(1);
            A.CallTo(() => documentService.SearchDocuments<Document>("test")).Returns(
                searchResultModels);

            var searchResults = searchController.GetSearchResults("test", null);

            searchResults.Data.As<IEnumerable<SearchResultModel>>().Should().BeEquivalentTo(searchResultModels);
        }
    }
}