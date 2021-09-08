using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using MrCMS.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models.ContentBlocks;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class ContentBlockController : MrCMSAdminController
    {
        private readonly IContentBlockAdminService _contentBlockAdminService;
        private readonly IModelBindingHelperAdapter _modelBindingHelperAdapter;

        public ContentBlockController(IContentBlockAdminService contentBlockAdminService,
            IModelBindingHelperAdapter modelBindingHelperAdapter)
        {
            _contentBlockAdminService = contentBlockAdminService;
            _modelBindingHelperAdapter = modelBindingHelperAdapter;
        }

        [HttpGet]
        public ViewResult Add(AddContentBlockViewModel addModel)
        {
            ViewData["additional-property-model"] =
                _contentBlockAdminService.GetAdditionalPropertyModel(addModel.BlockType);
            return View(addModel);
        }

        [HttpPost, ActionName(nameof(Add))]
        public async Task<RedirectToActionResult> Add_POST(AddContentBlockViewModel addModel)
        {
            var additionalPropertyModel = _contentBlockAdminService.GetAdditionalPropertyModel(addModel.BlockType);
            if (additionalPropertyModel != null)
            {
                await _modelBindingHelperAdapter.TryUpdateModelAsync(this, additionalPropertyModel,
                    additionalPropertyModel.GetType(), string.Empty);
            }

            var id = await _contentBlockAdminService.Add(addModel, additionalPropertyModel);

            if (id != null)
            {
                return RedirectToAction("Edit", "Webpage", new {id});
            }

            return RedirectToAction("Index", "Webpage");
        }

        [HttpGet]
        public async Task<ViewResult> Edit(int id)
        {
            ViewData["block"] = await _contentBlockAdminService.GetEntity(id);
            return View(await _contentBlockAdminService.GetUpdateModel(id));
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(UpdateContentBlockViewModel updateModel)
        {
            var additionalPropertyModel = await _contentBlockAdminService.GetAdditionalPropertyModel(updateModel.Id);
            if (additionalPropertyModel != null)
            {
                await _modelBindingHelperAdapter.TryUpdateModelAsync(this, additionalPropertyModel,
                    additionalPropertyModel.GetType(), string.Empty);
            }

            var id = await _contentBlockAdminService.Update(updateModel, additionalPropertyModel);

            if (id != null)
            {
                return RedirectToAction("Edit", "Webpage", new {id});
            }

            return RedirectToAction("Index", "Webpage");
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Delete(int id)
        {
            var webpageId = await _contentBlockAdminService.Delete(id);

            if (webpageId != null)
            {
                return RedirectToAction("Edit", "Webpage", new {id = webpageId});
            }

            return RedirectToAction("Index", "Webpage");
        }


        [HttpGet]
        public async Task<ViewResult> Sort(int id)
        {
            var sortItems = await _contentBlockAdminService.GetSortItems(id);
            ViewData["web-page"] = id;

            return View(sortItems);
        }

        [HttpPost]
        public async Task<ActionResult> Sort(IList<SortItem> sortItems, int webpageId)
        {
            await _contentBlockAdminService.Sort(sortItems);

            return RedirectToAction("Edit", "Webpage", new {id = webpageId});
        }
    }
}