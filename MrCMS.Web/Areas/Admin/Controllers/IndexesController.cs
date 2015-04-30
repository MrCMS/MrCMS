using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Indexing.Management;
using MrCMS.Search;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class IndexesController : MrCMSAdminController
    {
        private readonly IIndexAdminService _indexAdminService;

        public IndexesController(IIndexAdminService indexAdminService)
        {
            _indexAdminService = indexAdminService;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(IndexACL), IndexACL.View)]
        public ViewResult Index()
        {
            ViewData["universal-search-index-info"] = _indexAdminService.GetUniversalSearchIndexInfo();
            return View(_indexAdminService.GetIndexes());
        }

        [HttpPost]
        [MrCMSACLRule(typeof(IndexACL), IndexACL.Reindex)]
        public RedirectToRouteResult Reindex(string type)
        {
            _indexAdminService.Reindex(type);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MrCMSACLRule(typeof(IndexACL), IndexACL.Create)]
        public RedirectToRouteResult Create(string type)
        {
            _indexAdminService.Reindex(type);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MrCMSACLRule(typeof(IndexACL), IndexACL.Optimize)]
        public RedirectToRouteResult Optimise(string type)
        {
            _indexAdminService.Optimise(type);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [MrCMSACLRule(typeof(IndexACL), IndexACL.SetBoosts)]
        public ViewResult Boosts(string type)
        {
            return View(_indexAdminService.GetBoosts(type));
        }

        [HttpPost]
        [MrCMSACLRule(typeof(IndexACL), IndexACL.SetBoosts)]
        public RedirectToRouteResult Boosts(List<LuceneFieldBoost> boosts)
        {
            _indexAdminService.SaveBoosts(boosts);
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [MrCMSACLRule(typeof(IndexACL), IndexACL.Reindex)]
        public RedirectToRouteResult ReindexUniversalSearch()
        {
            _indexAdminService.ReindexUniversalSearch();
            TempData.SuccessMessages().Add("Univeral Search reindexed");
            return RedirectToAction("Index");
        }

        public ActionResult OptimiseUniversalSearch()
        {
            _indexAdminService.OptimiseUniversalSearch();
            TempData.SuccessMessages().Add("Univeral Search optimised");
            return RedirectToAction("Index");
        }
    }
}