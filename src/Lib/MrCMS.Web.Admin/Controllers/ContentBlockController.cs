using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Admin.Models.ContentBlocks;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class ContentBlockController : MrCMSAdminController
    {
        private readonly IContentBlockAdminService _contentBlockAdminService;
        private readonly IModelBindingHelperAdapter _modelBindingHelperAdapter;

        public ContentBlockController(IContentBlockAdminService contentBlockAdminService, IModelBindingHelperAdapter modelBindingHelperAdapter)
        {
            _contentBlockAdminService = contentBlockAdminService;
            _modelBindingHelperAdapter = modelBindingHelperAdapter;
        }

        [HttpGet]
        public ViewResult Add(AddContentBlockViewModel addModel)
        {
            ViewData["additional-property-model"] = _contentBlockAdminService.GetAdditionalPropertyModel(addModel.BlockType);
            return View(addModel);
        }

        [HttpPost, ActionName(nameof(Add))]
        public async Task<RedirectToActionResult> Add_POST(AddContentBlockViewModel addModel)
        {
            var additionalPropertyModel = _contentBlockAdminService.GetAdditionalPropertyModel(addModel.BlockType);
            if (additionalPropertyModel != null)
            {
                await _modelBindingHelperAdapter.TryUpdateModelAsync(this, additionalPropertyModel, additionalPropertyModel.GetType(), string.Empty);
            }

            var id = _contentBlockAdminService.Add(addModel, additionalPropertyModel);

            if (id != null)
            {
                return RedirectToAction("Edit", "Webpage", new { id });
            }

            return RedirectToAction("Index", "Webpage");
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            ViewData["block"] = _contentBlockAdminService.GetEntity(id);
            return View(_contentBlockAdminService.GetUpdateModel(id));
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(UpdateContentBlockViewModel updateModel)
        {
            var additionalPropertyModel = _contentBlockAdminService.GetAdditionalPropertyModel(updateModel.Id);
            if (additionalPropertyModel != null)
            {
                await _modelBindingHelperAdapter.TryUpdateModelAsync(this, additionalPropertyModel, additionalPropertyModel.GetType(), string.Empty);
            }

            var id = _contentBlockAdminService.Update(updateModel, additionalPropertyModel);

            if (id != null)
            {
                return RedirectToAction("Edit", "Webpage", new { id });
            }

            return RedirectToAction("Index", "Webpage");
        }

        [HttpGet]
        public ViewResult Delete(int id)
        {
            return View(_contentBlockAdminService.GetUpdateModel(id));
        }

        [HttpPost, ActionName(nameof(Delete))]
        public RedirectToActionResult Delete_POST(int id)
        {
            var webpageId = _contentBlockAdminService.Delete(id);

            if (webpageId != null)
            {
                return RedirectToAction("Edit", "Webpage", new { id = webpageId });
            }

            return RedirectToAction("Index", "Webpage");
        }


        [HttpGet]
        public ViewResult Sort(int id)
        {
            var sortItems = _contentBlockAdminService.GetSortItems(id);
            ViewData["web-page"] = id;

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult Sort(IList<SortItem> sortItems, int webpageId)
        {
            _contentBlockAdminService.Sort(sortItems);

            return RedirectToAction("Edit", "Webpage", new { id = webpageId });
        }

    }
}