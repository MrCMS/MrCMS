using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Infrastructure.Helpers;
using MrCMS.Web.Admin.Models.Forms;

namespace MrCMS.Web.Admin.Controllers
{
    public class FormController : MrCMSAdminController
    {
        private readonly IFormAdminService _formAdminService;

        public FormController(IFormAdminService formAdminService)
        {
            _formAdminService = formAdminService;
        }

        public async Task<ViewResult> Index(FormSearchModel model)
        {
            ViewData["results"] = await _formAdminService.Search(model);

            return View(model);
        }

        public ViewResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Add(AddFormModel model)
        {
            var form = await _formAdminService.AddForm(model);
            if (form != null)
            {
                return RedirectToAction("Edit", new {form.Id});
            }

            return RedirectToAction("Index");
        }

        public async Task<ViewResult> Edit(int id)
        {
            var form = await _formAdminService.GetForm(id);
            ViewData["form"] = form;
            return View(form);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(UpdateFormModel model)
        {
            await _formAdminService.Update(model);

            TempData.AddSuccessMessage($"'{model.Name}' updated");

            return RedirectToAction("Edit", new {model.Id});
        }

        public async Task<ViewResult> Delete(int id)
        {
            return View(await _formAdminService.GetUpdateModel(id));
        }

        public async Task<ViewResult> PagesWithForms(WebpagesWithEmbeddedFormQuery query)
        {
            ViewData["results"] = await _formAdminService.GetPagesWithForms(query);
            
            return View(query);
        }

        [HttpPost, ActionName(nameof(Delete))]
        public async Task<RedirectToActionResult> Delete_POST(int id)
        {
            await _formAdminService.Delete(id);
            return RedirectToAction("Index");
        }

        public async Task<PartialViewResult> Postings(int id, int page = 1, string search = null)
        {
            var form = await _formAdminService.GetForm(id);
            var data = await _formAdminService.GetFormPostings(form, page, search);

            return PartialView(data);
        }

        public async Task<ActionResult> ViewPosting(int id)
        {
            var formPosting = await _formAdminService.GetFormPosting(id);
            return PartialView(formPosting);
        }

        [HttpGet]
        public async Task<ActionResult> Sort(int id)
        {
            var form = await _formAdminService.GetForm(id);
            var sortItems = form.FormProperties.OrderBy(x => x.DisplayOrder)
                .Select(
                    arg => new SortItem {Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name})
                .ToList();

            return View(sortItems);
        }

        [HttpPost]
        public async Task Sort(List<SortItem> items)
        {
            await _formAdminService.SetOrders(items);
        }

        [HttpGet]
        public async Task<PartialViewResult> ClearFormData(int id)
        {
            var form = await _formAdminService.GetForm(id);
            return PartialView(form);
        }

        [HttpPost]
        [ActionName("ClearFormData")]
        public async Task<RedirectToActionResult> ClearFormData_POST(int id)
        {
            var form = await _formAdminService.GetForm(id);
            await _formAdminService.ClearFormData(form);
            return RedirectToAction("Edit", "Form", new {id = form.Id});
        }

        [HttpGet]
        public async Task<ActionResult> ExportFormData(int id)
        {
            var form = await _formAdminService.GetForm(id);
            try
            {
                var file = await _formAdminService.ExportFormData(form);
                return File(file, "text/csv", "MrCMS-FormData-[" + form.Name + "]-" + DateTime.UtcNow + ".csv");
            }
            catch
            {
                return RedirectToAction("Edit", "Form", new {id = form.Id});
            }
        }

        [HttpGet]
        public async Task<ViewResult> DeleteEntry(int id)
        {
            var formPosting = await _formAdminService.GetFormPosting(id);
            return View(formPosting);
        }

        [HttpPost]
        [ActionName("DeleteEntry")]
        public async Task<ActionResult> DeleteEntry_POST(int id)
        {
            var posting = await _formAdminService.DeletePosting(id);
            return RedirectToAction("Edit", "Form", new {id = posting.Form.Id});
        }

        [HttpGet]
        public async Task<PartialViewResult> FormProperties(int id)
        {
            return PartialView(await _formAdminService.GetForm(id));
        }
    }
}