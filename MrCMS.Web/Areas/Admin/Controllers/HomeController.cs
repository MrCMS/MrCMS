using System;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class HomeController : MrCMSAdminController
    {
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly IUserService _userServices;
        private readonly ISession _session;

        public HomeController(ICurrentSiteLocator currentSiteLocator, IUserService userServices, ISession session)
        {
            _currentSiteLocator = currentSiteLocator;
            _userServices = userServices;
            _session = session;
        }

        public ActionResult Index()
        {
            WebpageStats countAlias = null;
            Webpage webpageAlias = null;
            var currentSite = _currentSiteLocator.GetCurrentSite();
            var list = _session.QueryOver(() => webpageAlias)
                .Where(x => x.Site == currentSite)
                       .SelectList(
                           builder =>
                           builder.SelectGroup(() => webpageAlias.DocumentType)
                                  .WithAlias(() => countAlias.DocumentType)
                                  .SelectCount(() => webpageAlias.Id)
                                  .WithAlias(() => countAlias.NumberOfPages)
                                  .SelectSubQuery(QueryOver.Of<Webpage>().Where(webpage => webpage.Site == currentSite && webpage.DocumentType == webpageAlias.DocumentType && (webpage.PublishOn == null || webpage.PublishOn > CurrentRequestData.Now)).ToRowCountQuery())
                                  .WithAlias(() => countAlias.NumberOfUnPublishedPages))
                       .TransformUsing(Transformers.AliasToBean<WebpageStats>())
                       .List<WebpageStats>();

            var model = new Dashboard
                            {
                                SiteName = currentSite.Name.Trim(),
                                LoggedInName = _userServices.GetCurrentUser(HttpContext).FirstName,
                                Stats = list,
                                ActiveUsers = _userServices.ActiveUsers(),
                                NoneActiveUsers = _userServices.NonActiveUsers()
                            };

            return View(model);
        }
    }
}