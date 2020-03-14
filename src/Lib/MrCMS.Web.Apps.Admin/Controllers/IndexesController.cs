using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Models;
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
            ViewData["universal-search-index-info"] = _indexAdminService.GetUniversalSearchIndexInfo();
            return View(_indexAdminService.GetIndexes());
        }

        [HttpPost]
        [Acl(typeof(IndexACL), IndexACL.Reindex)]
        public async Task<RedirectToActionResult> Reindex(string type)
        {
            await _indexAdminService.Reindex(type);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Acl(typeof(IndexACL), IndexACL.Create)]
        public async Task<RedirectToActionResult> Create(string type)
        {
            await _indexAdminService.Reindex(type);
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
        public async Task<RedirectToActionResult> Boosts(List<UpdateLuceneFieldBoostModel> boosts)
        {
            await _indexAdminService.SaveBoosts(boosts);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Acl(typeof(IndexACL), IndexACL.Reindex)]
        public async Task<RedirectToActionResult> ReindexUniversalSearch()
        {
            await _indexAdminService.ReindexUniversalSearch();
            TempData.SuccessMessages().Add("Univeral Search reindexed");
            return RedirectToAction("Index");
        }
    }
}