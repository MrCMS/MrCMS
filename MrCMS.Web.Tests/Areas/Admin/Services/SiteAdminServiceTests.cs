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
using MrCMS.Website;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class SiteAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly ICloneSiteService _cloneSiteService;
        private readonly SiteAdminService _siteService;

        public SiteAdminServiceTests()
        {
            _cloneSiteService = A.Fake<ICloneSiteService>();
            _siteService = new SiteAdminService(Session, _cloneSiteService);
        }

        [Fact]
        public void SiteAdminService_GetAllSites_ReturnsPersistedSites()
        {
            List<Site> sites = Enumerable.Range(1, 10).Select(i => new Site {Name = "Site " + i}).ToList();
            sites.ForEach(site => Session.Transact(session => session.Save(site)));

            List<Site> allSites = _siteService.GetAllSites();

            sites.ForEach(site => allSites.Should().Contain(site));
        }

        [Fact]
        public void SiteAdminService_AddSite_ShouldPersistSiteToSession()
        {
            var user = new User();
            Session.Transact(session => session.Save(user));
            CurrentRequestData.CurrentUser = user;
            var site = new Site();
            var options = new List<SiteCopyOption>();

            _siteService.AddSite(site, options);

            // Including CurrentSite from the base class
            Session.QueryOver<Site>().RowCount().Should().Be(2);
        }

        [Fact]
        public void SiteAdminService_AddSite_SavesPassedSiteToSession()
        {
            var site = new Site();
            var options = new List<SiteCopyOption>();

            _siteService.AddSite(site, options);

            Session.QueryOver<Site>().List().Should().Contain(site);
        }


        [Fact]
        public void SiteAdminService_SaveSite_UpdatesPassedSite()
        {
            var site = new Site();
            Session.Transact(session => session.Save(site));
            site.Name = "updated";

            _siteService.SaveSite(site);

            Session.Evict(site);
            Session.QueryOver<Site>().Where(s => s.Name == "updated").RowCount().Should().Be(1);
        }

        [Fact]
        public void SiteAdminService_DeleteSite_ShouldDeleteSiteFromSession()
        {
            var site = new Site();
            Session.Transact(session => session.Save(site));

            _siteService.DeleteSite(site);

            Session.QueryOver<Site>().List().Should().NotContain(site);
        }

        [Fact]
        public void SiteAdminService_DeleteSite_ShouldRemoveSiteFromSession()
        {
            _siteService.DeleteSite(CurrentSite);

            // Including CurrentSite from the base class
            Session.QueryOver<Site>().RowCount().Should().Be(0);
        }

        [Fact]
        public void SiteAdminService_GetSite_ReturnsResultFromSessionGetAsResult()
        {
            var site = new Site();
            Session.Transact(session => session.Save(site));

            _siteService.GetSite(site.Id).Should().Be(site);
        }
    }
}