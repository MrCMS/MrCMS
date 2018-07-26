using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class IndexesController : MrCMSAdminController
    {
        private readonly IIndexAdminService _indexAdminService;

        public IndexesController(IIndexAdminService indexAdminService)
        {
            _indexAdminService = indexAdminService;
        }

        [HttpGet]
        [Acl(typeof(IndexACL), IndexACL.View)]
        public ViewResult Index()
        {
            // TODO: universal index
            //ViewData["universal-search-index-info"] = _indexAdminService.GetUniversalSearchIndexInfo();
            return View(_indexAdminService.GetIndexes());
        }

        [HttpPost]
        [Acl(typeof(IndexACL), IndexACL.Reindex)]
        public RedirectToActionResult Reindex(string type)
        {
            _indexAdminService.Reindex(type);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Acl(typeof(IndexACL), IndexACL.Create)]
        public RedirectToActionResult Create(string type)
        {
            _indexAdminService.Reindex(type);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Acl(typeof(IndexACL), IndexACL.Optimize)]
        public RedirectToActionResult Optimise(string type)
        {
            _indexAdminService.Optimise(type);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Acl(typeof(IndexACL), IndexACL.SetBoosts)]
        public ViewResult Boosts(string type)
        {
            return View(_indexAdminService.GetBoosts(type));
        }

        [HttpPost]
        [Acl(typeof(IndexACL), IndexACL.SetBoosts)]
        public RedirectToActionResult Boosts(List<LuceneFieldBoost> boosts)
        {
            _indexAdminService.SaveBoosts(boosts);
            return RedirectToAction("Index");
        }
        // TODO: universal index
        
        //[HttpPost]
        //[Acl(typeof(IndexACL), IndexACL.Reindex)]
        //public RedirectToActionResult ReindexUniversalSearch()
        //{
        //    _indexAdminService.ReindexUniversalSearch();
        //    TempData.SuccessMessages().Add("Univeral Search reindexed");
        //    return RedirectToAction("Index");
        //}

        //public ActionResult OptimiseUniversalSearch()
        //{
        //    _indexAdminService.OptimiseUniversalSearch();
        //    TempData.SuccessMessages().Add("Univeral Search optimised");
        //    return RedirectToAction("Index");
        //}
    }
}