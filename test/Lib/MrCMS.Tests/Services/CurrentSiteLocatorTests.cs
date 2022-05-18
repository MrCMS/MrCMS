using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.TestSupport;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class CurrentSiteLocatorTests
    {
        //private HttpRequestBase _httpRequestBase;
        private ContextCurrentSiteLocator _contextCurrentSiteLocator;
        private InMemoryRepository<Site> _inMemoryRepository;
        private InMemoryRepository<RedirectedDomain> _domainRepository;
        private readonly Site CurrentSite;
        private IHttpContextAccessor _contextAccessor;

        public CurrentSiteLocatorTests()
        {
            CurrentSite = new Site { Name = "Current Site", BaseUrl = "www.currentsite.com", Id = 1 };
            //_httpRequestBase = A.Fake<HttpRequestBase>();
            _contextAccessor = A.Fake<IHttpContextAccessor>();
            _inMemoryRepository = new InMemoryRepository<Site>();
            _domainRepository = new InMemoryRepository<RedirectedDomain>();
            _contextCurrentSiteLocator = new ContextCurrentSiteLocator(_inMemoryRepository,
                _domainRepository, _contextAccessor);
        }

        // [Fact]
        // public async Task CurrentSiteLocator_GetCurrentSite_ReturnsFirstIfNoneMatch()
        // {
        //     await _inMemoryRepository.Add(CurrentSite);
        //     var site1 = new Site { BaseUrl = "test1" };
        //     var site2 = new Site { BaseUrl = "test2" };
        //     await _inMemoryRepository.Add(site1);
        //     await _inMemoryRepository.Add(site2);
        //     A.CallTo(() => _contextAccessor.HttpContext.Request.Host).Returns(new HostString("www.example.com"));
        //
        //     _contextCurrentSiteLocator.GetCurrentSite().Should().Be(CurrentSite);
        // }

        [Fact]
        public async Task CurrentSiteLocator_GetCurrentSite_IfUrlMatchesReturnsMatchingSite()
        {
            await _inMemoryRepository.Add(CurrentSite);
            var site1 = new Site { BaseUrl = "test1" };
            var site2 = new Site { BaseUrl = "www.example.com" };
            await _inMemoryRepository.Add(site1);
            await _inMemoryRepository.Add(site2);
            A.CallTo(() => _contextAccessor.HttpContext.Request.Host).Returns(new HostString("www.example.com"));

            _contextCurrentSiteLocator.GetCurrentSite().Should().Be(site2);
        }
    }
}