using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services.CloneSite;
using MrCMS.TestSupport;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Services
{
    public class SiteAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly ICloneSiteService _cloneSiteService;
        private readonly SiteAdminService _siteService;
        private readonly IMapper _mapper;

        public SiteAdminServiceTests()
        {
            _cloneSiteService = A.Fake<ICloneSiteService>();
            _mapper = A.Fake<IMapper>();
            _siteService = new SiteAdminService(Session, _cloneSiteService, _mapper);
        }

        [Fact]
        public void SiteAdminService_GetAllSites_ReturnsPersistedSites()
        {
            List<Site> sites = Enumerable.Range(1, 10).Select(i => new Site { Name = "Site " + i }).ToList();
            sites.ForEach(site => Session.Transact(session => session.Save(site)));

            List<Site> allSites = _siteService.GetAllSites();

            sites.ForEach(site => allSites.Should().Contain(site));
        }

        [Fact]
        public void SiteAdminService_AddSite_ShouldPersistSiteToSession()
        {
            var user = new User();
            Session.Transact(session => session.Save(user));
            //CurrentRequestData.CurrentUser = user;
            AddSiteModel model = new AddSiteModel();
            var site = new Site();
            A.CallTo(() => _mapper.Map<Site>(model)).Returns(site);
            var options = new List<SiteCopyOption>();

            _siteService.AddSite(model, options);

            // Including CurrentSite from the base class
            Session.QueryOver<Site>().RowCount().Should().Be(2);
        }

        [Fact]
        public void SiteAdminService_AddSite_SavesPassedSiteToSession()
        {
            AddSiteModel model = new AddSiteModel();
            var site = new Site();
            A.CallTo(() => _mapper.Map<Site>(model)).Returns(site);
            var options = new List<SiteCopyOption>();

            _siteService.AddSite(model, options);

            Session.QueryOver<Site>().List().Should().Contain(site);
        }


        [Fact]
        public void SiteAdminService_SaveSite_UpdatesPassedSite()
        {
            var site = new Site();
            Session.Transact(session => session.Save(site));
            site.Name = "updated";
            var updateSiteModel = new UpdateSiteModel{Id=site.Id};

            _siteService.SaveSite(updateSiteModel);

            A.CallTo(() => _mapper.Map(updateSiteModel, site)).MustHaveHappened();
            Session.Evict(site);
            Session.QueryOver<Site>().Where(s => s.Name == "updated").RowCount().Should().Be(1);
        }

        [Fact]
        public void SiteAdminService_DeleteSite_ShouldDeleteSiteFromSession()
        {
            var site = new Site();
            Session.Transact(session => session.Save(site));

            _siteService.DeleteSite(site.Id);

            Session.QueryOver<Site>().List().Should().NotContain(site);
        }

        [Fact]
        public void SiteAdminService_DeleteSite_ShouldRemoveSiteFromSession()
        {
            _siteService.DeleteSite(CurrentSite.Id);

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