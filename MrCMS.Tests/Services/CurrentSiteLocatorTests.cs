using System;
using System.Web;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class CurrentSiteLocatorTests : InMemoryDatabaseTest
    {
        private HttpRequestBase _httpRequestBase;
        private CurrentSiteLocator _currentSiteLocator;

        public CurrentSiteLocatorTests()
        {
            _httpRequestBase = A.Fake<HttpRequestBase>();
            _currentSiteLocator = new CurrentSiteLocator(Session, _httpRequestBase);
        }

        [Fact]
        public void CurrentSiteLocator_GetCurrentSite_ReturnsFirstIfNoneMatch()
        {
            var site1 = new Site { BaseUrl = "test1" };
            var site2 = new Site { BaseUrl = "test2" };
            Session.Transact(session =>
                                 {
                                     session.Save(site1);
                                     session.Save(site2);
                                 });
            A.CallTo(() => _httpRequestBase.Url).Returns(new Uri("http://www.example.com/"));

            _currentSiteLocator.GetCurrentSite().Should().Be(CurrentSite);
        }

        [Fact]
        public void CurrentSiteLocator_GetCurrentSite_IfUrlMatchesReturnsMatchingSite()
        {
            var site1 = new Site { BaseUrl = "test1" };
            var site2 = new Site { BaseUrl = "www.example.com" };
            Session.Transact(session =>
                                 {
                                     session.Save(site1);
                                     session.Save(site2);
                                 });
            A.CallTo(() => _httpRequestBase.Url).Returns(new Uri("http://www.example.com/"));

            _currentSiteLocator.GetCurrentSite().Should().Be(site2);
        }
    }
}