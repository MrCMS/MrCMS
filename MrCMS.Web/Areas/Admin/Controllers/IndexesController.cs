using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class IndexesController : MrCMSAdminController
    {
        private readonly IIndexService _indexService;
        private readonly IIndexAdminService _indexAdminService;

        public IndexesController(IIndexService indexService, IIndexAdminService indexAdminService)
        {
            _indexService = indexService;
            _indexAdminService = indexAdminService;
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
    }
}