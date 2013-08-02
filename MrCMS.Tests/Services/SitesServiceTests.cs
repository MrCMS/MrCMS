using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services
{
    public class SitesServiceTests : InMemoryDatabaseTest
    {
        private readonly ICloneSiteService _cloneSiteService;
        private readonly IIndexService _indexService;
        private readonly SiteService _siteService;

        public SitesServiceTests()
        {
            _cloneSiteService = A.Fake<ICloneSiteService>();
            _indexService = A.Fake<IIndexService>();
            _siteService = new SiteService(Session, _cloneSiteService,_indexService);
        }

        [Fact]
        public void SitesService_GetAllSites_ReturnsPersistedSites()
        {
            var sites = Enumerable.Range(1, 10).Select(i => new Site { Name = "Site " + i }).ToList();
            sites.ForEach(site => Session.Transact(session => session.Save(site)));

            var allSites = _siteService.GetAllSites();

            sites.ForEach(site => allSites.Should().Contain(site));
        }

        [Fact]
        public void SitesService_AddSite_ShouldPersistSiteToSession()
        {
            var user = new User();
            Session.Transact(session => session.Save(user));
            CurrentRequestData.CurrentUser = user;
            var site = new Site();
            var options = new SiteCopyOptions();

            _siteService.AddSite(site, options);

            // Including CurrentSite from the base class
            Session.QueryOver<Site>().RowCount().Should().Be(2);
        }

        [Fact]
        public void SitesService_AddSite_SavesPassedSiteToSession()
        {
            var site = new Site();
            var options = new SiteCopyOptions();

            _siteService.AddSite(site, options);

            Session.QueryOver<Site>().List().Should().Contain(site);
        }


        [Fact]
        public void SitesService_SaveSite_UpdatesPassedSite()
        {
            var site = new Site();
            Session.Transact(session => session.Save(site));
            site.Name = "updated";

            _siteService.SaveSite(site);

            Session.Evict(site);
            Session.QueryOver<Site>().Where(s => s.Name == "updated").RowCount().Should().Be(1);
        }

        [Fact]
        public void SitesService_DeleteSite_ShouldDeleteSiteFromSession()
        {
            var site = new Site();
            Session.Transact(session => session.Save(site));

            _siteService.DeleteSite(site);

            Session.QueryOver<Site>().List().Should().NotContain(site);
        }

        [Fact]
        public void SitesService_DeleteSite_ShouldRemoveSiteFromSession()
        {
            _siteService.DeleteSite(CurrentSite);

            // Including CurrentSite from the base class
            Session.QueryOver<Site>().RowCount().Should().Be(0);
        }

        [Fact]
        public void SitesService_GetSite_ReturnsResultFromSessionGetAsResult()
        {
            var site = new Site();
            Session.Transact(session => session.Save(site));

            _siteService.GetSite(site.Id).Should().Be(site);
        }

    }
}