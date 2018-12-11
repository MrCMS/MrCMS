using System;
using System.Web;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.TestSupport;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class CurrentSiteLocatorTests
    {
        //private HttpRequestBase _httpRequestBase;
        private CurrentSiteLocator _currentSiteLocator;
        private InMemoryRepository<Site> _inMemoryRepository;
        private readonly Site CurrentSite;
        private IHttpContextAccessor _contextAccessor;

        public CurrentSiteLocatorTests()
        {
            CurrentSite = new Site { Name = "Current Site", BaseUrl = "www.currentsite.com", Id = 1 };
            //_httpRequestBase = A.Fake<HttpRequestBase>();
            _contextAccessor = A.Fake<IHttpContextAccessor>();
            _inMemoryRepository = new InMemoryRepository<Site>();
            _currentSiteLocator = new CurrentSiteLocator(_inMemoryRepository, _contextAccessor);
            _inMemoryRepository.Add(CurrentSite);
        }

        [Fact]
        public void CurrentSiteLocator_GetCurrentSite_ReturnsFirstIfNoneMatch()
        {
            var site1 = new Site { BaseUrl = "test1" };
            var site2 = new Site { BaseUrl = "test2" };
            _inMemoryRepository.Add(site1);
            _inMemoryRepository.Add(site2);
            A.CallTo(() => _contextAccessor.HttpContext.Request.Host).Returns(new HostString("www.example.com"));

            _currentSiteLocator.GetCurrentSite().Should().Be(CurrentSite);
        }

        [Fact]
        public void CurrentSiteLocator_GetCurrentSite_IfUrlMatchesReturnsMatchingSite()
        {
            var site1 = new Site { BaseUrl = "test1" };
            var site2 = new Site { BaseUrl = "www.example.com" };
            _inMemoryRepository.Add(site1);
            _inMemoryRepository.Add(site2);
            A.CallTo(() => _contextAccessor.HttpContext.Request.Host).Returns(new HostString("www.example.com"));

            _currentSiteLocator.GetCurrentSite().Should().Be(site2);
        }
    }
}