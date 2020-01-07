using System;
using System.Collections.Generic;
using System.Web;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MrCMS.Data;
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
        private IGlobalRepository<Site> _repository;
        private readonly Site CurrentSite;
        private IHttpContextAccessor _contextAccessor;
        private IConfiguration _configuration;

        public CurrentSiteLocatorTests()
        {
            CurrentSite = new Site { Name = "Current Site", BaseUrl = "www.currentsite.com", Id = 1 };
            //_httpRequestBase = A.Fake<HttpRequestBase>();
            _contextAccessor = A.Fake<IHttpContextAccessor>();
            _repository = A.Fake<IGlobalRepository<Site>>();
            _configuration = A.Fake<IConfiguration>();
            _currentSiteLocator = new CurrentSiteLocator(_repository, _contextAccessor, _configuration);
        }

        [Fact]
        public void CurrentSiteLocator_GetCurrentSite_ReturnsFirstIfNoneMatch()
        {
            var site1 = new Site { BaseUrl = "test1" };
            var site2 = new Site { BaseUrl = "test2" };
            A.CallTo(() => _repository.Query()).Returns(new List<Site> {CurrentSite, site1, site2}.AsAsyncQueryable());
            A.CallTo(() => _contextAccessor.HttpContext.Request.Host).Returns(new HostString("www.example.com"));

            _currentSiteLocator.GetCurrentSite().Should().Be(CurrentSite);
        }

        [Fact]
        public void CurrentSiteLocator_GetCurrentSite_IfUrlMatchesReturnsMatchingSite()
        {
            var site1 = new Site { BaseUrl = "test1" };
            var site2 = new Site { BaseUrl = "www.example.com" };
            A.CallTo(() => _repository.Query()).Returns(new List<Site> {CurrentSite, site1, site2}.AsAsyncQueryable());
            A.CallTo(() => _contextAccessor.HttpContext.Request.Host).Returns(new HostString("www.example.com"));

            _currentSiteLocator.GetCurrentSite().Should().Be(site2);
        }
    }
}