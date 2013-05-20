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
        private readonly IUserService _userServices;
        private readonly ISiteService _siteService;
        private readonly ISession _session;

        public HomeController(ISiteService siteService, IUserService userServices, ISession session)
        {
            _siteService = siteService;
            _userServices = userServices;
            _session = session;
        }

        public ActionResult Index()
        {
            WebpageStats countAlias = null;
            Webpage webpageAlias = null;
            var list = _session.QueryOver(() => webpageAlias)
                .Where(x => x.Site == _siteService.GetCurrentSite())
                       .SelectList(
                           builder =>
                           builder.SelectGroup(() => webpageAlias.DocumentType)
                                  .WithAlias(() => countAlias.DocumentType)
                                  .SelectCount(() => webpageAlias.Id)
                                  .WithAlias(() => countAlias.NumberOfPages)
                                  .SelectSubQuery(QueryOver.Of<Webpage>().Where(webpage => webpage.DocumentType == webpageAlias.DocumentType && (webpage.PublishOn == null || webpage.PublishOn > CurrentRequestData.Now)).ToRowCountQuery())
                                  .WithAlias(() => countAlias.NumberOfUnPublishedPages))
                       .TransformUsing(Transformers.AliasToBean<WebpageStats>())
                       .List<WebpageStats>();

            var model = new Dashboard
                            {
                                SiteName = _siteService.GetCurrentSite().Name.Trim(),
                                LoggedInName = _userServices.GetCurrentUser(HttpContext).FirstName,
                                Stats = list,
                                ActiveUsers = _userServices.ActiveUsers(),
                                NoneActiveUsers = _userServices.NoneActiveUsers()
                            };

            return View(model);
        }
    }
}