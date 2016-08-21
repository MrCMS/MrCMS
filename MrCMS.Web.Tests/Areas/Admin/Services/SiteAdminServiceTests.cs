using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Services.CloneSite;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.TestSupport;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class SiteAdminServiceTests 
    {
        private readonly ICloneSiteService _cloneSiteService;
        private readonly SiteAdminService _siteService;
        private InMemoryRepository<Site> _siteRepository = new InMemoryRepository<Site>();

        public SiteAdminServiceTests()
        {
            _cloneSiteService = A.Fake<ICloneSiteService>();
            _siteService = new SiteAdminService(_siteRepository, _cloneSiteService);
        }

        [Fact]
        public void SiteAdminService_GetAllSites_ReturnsPersistedSites()
        {
            List<Site> sites = Enumerable.Range(1, 10).Select(i => new Site {Name = "Site " + i}).ToList();
            sites.ForEach(_siteRepository.Add);

            List<Site> allSites = _siteService.GetAllSites();

            sites.ForEach(site => allSites.Should().Contain(site));
        }

        [Fact]
        public void SiteAdminService_AddSite_ShouldPersistSiteToSession()
        {
            var site = new Site();
            var options = new List<SiteCopyOption>();

            _siteService.AddSite(site, options);

            _siteRepository.Query().Count().Should().Be(1);
        }

        [Fact]
        public void SiteAdminService_AddSite_SavesPassedSiteToSession()
        {
            var site = new Site();
            var options = new List<SiteCopyOption>();

            _siteService.AddSite(site, options);

            _siteRepository.Query().Should().Contain(site);
        }


        [Fact]
        public void SiteAdminService_UpdateSite_UpdatesPassedSite()
        {
            var site = new Site();
            _siteRepository.Add(site);
            site.Name = "updated";

            _siteService.UpdateSite(site);

            _siteRepository.Query().Count(s => s.Name == "updated").Should().Be(1);
        }

        [Fact]
        public void SiteAdminService_DeleteSite_ShouldDeleteSiteFromSession()
        {
            var site = new Site();
            _siteRepository.Add(site);

            _siteService.DeleteSite(site);

            _siteRepository.Query().Should().NotContain(site);
        }


        [Fact]
        public void SiteAdminService_GetSite_ReturnsResultFromSessionGetAsResult()
        {
            var site = new Site();
            _siteRepository.Add(site);

            _siteService.GetSite(site.Id).Should().Be(site);
        }
    }
}