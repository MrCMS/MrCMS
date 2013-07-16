using System.Web.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class IndexesController : MrCMSAdminController
    {
        private readonly IIndexService _indexService;

        public IndexesController(IIndexService indexService)
        {
            _indexService = indexService;
        }
        [HttpGet]
        [MrCMSACLRule(typeof(IndexACL), IndexACL.View)]
        public ViewResult Index()
        {
            return View(_indexService.GetIndexes());
        }

        [HttpPost]
        [MrCMSACLRule(typeof(IndexACL), IndexACL.Reindex)]
        public RedirectToRouteResult Reindex(string type)
        {
            _indexService.Reindex(type);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MrCMSACLRule(typeof(IndexACL), IndexACL.Create)]
        public RedirectToRouteResult Create(string type)
        {
            _indexService.Reindex(type);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MrCMSACLRule(typeof(IndexACL), IndexACL.Optimize)]
        public RedirectToRouteResult Optimise(string type)
        {
            _indexService.Optimise(type);
            return RedirectToAction("Index");
        }
    }
}