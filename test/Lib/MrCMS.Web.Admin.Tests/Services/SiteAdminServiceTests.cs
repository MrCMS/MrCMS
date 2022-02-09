using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services.CloneSite;
using MrCMS.TestSupport;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Services
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
        public async Task SiteAdminService_GetAllSites_ReturnsPersistedSites()
        {
            List<Site> sites = Enumerable.Range(1, 10).Select(i => new Site {Name = "Site " + i}).ToList();
            foreach (var site in sites)
            {
                await Session.TransactAsync(session => session.SaveAsync(site));
            }

            IList<Site> allSites = await _siteService.GetAllSites();

            sites.ForEach(site => allSites.Should().Contain(site));
        }

        [Fact]
        public async Task SiteAdminService_AddSite_ShouldPersistSiteToSession()
        {
            var user = new User();
            await Session.TransactAsync(session => session.SaveAsync(user));
            //CurrentRequestData.CurrentUser = user;
            AddSiteModel model = new AddSiteModel();
            var site = new Site();
            A.CallTo(() => _mapper.Map<Site>(model)).Returns(site);
            var options = new List<SiteCopyOption>();

            await _siteService.AddSite(model, options);

            // Including CurrentSite from the base class
            (await Session.QueryOver<Site>().RowCountAsync()).Should().Be(2);
        }

        [Fact]
        public async Task SiteAdminService_AddSite_SavesPassedSiteToSession()
        {
            AddSiteModel model = new AddSiteModel();
            var site = new Site();
            A.CallTo(() => _mapper.Map<Site>(model)).Returns(site);
            var options = new List<SiteCopyOption>();

            await _siteService.AddSite(model, options);

            (await Session.QueryOver<Site>().ListAsync()).Should().Contain(site);
        }


        [Fact]
        public async Task SiteAdminService_SaveSite_UpdatesPassedSite()
        {
            var site = new Site();
            await Session.TransactAsync(session => session.SaveAsync(site));
            site.Name = "updated";
            var updateSiteModel = new UpdateSiteModel {Id = site.Id};

            await _siteService.SaveSite(updateSiteModel);

            A.CallTo(() => _mapper.Map(updateSiteModel, site)).MustHaveHappened();
            await Session.EvictAsync(site);
            (await Session.QueryOver<Site>().Where(s => s.Name == "updated").RowCountAsync()).Should().Be(1);
        }

        [Fact]
        public async Task SiteAdminService_DeleteSite_ShouldDeleteSiteFromSession()
        {
            var site = new Site();
            await Session.TransactAsync(session => session.SaveAsync(site));

            await _siteService.DeleteSite(site.Id);

            (await Session.QueryOver<Site>().ListAsync()).Should().NotContain(site);
        }

        [Fact]
        public async Task SiteAdminService_DeleteSite_ShouldRemoveSiteFromSession()
        {
            await _siteService.DeleteSite(CurrentSite.Id);

            // Including CurrentSite from the base class
            (await Session.QueryOver<Site>().RowCountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task SiteAdminService_GetSite_ReturnsResultFromSessionGetAsResult()
        {
            var site = new Site();
            await Session.TransactAsync(session => session.SaveAsync(site));

            _siteService.GetSite(site.Id).Should().Be(site);
        }
    }
}