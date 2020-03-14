using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Web.Apps.Admin.Helpers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class FormController : MrCMSAdminController
    {
        private readonly IFormAdminService _formAdminService;

        public FormController(IFormAdminService formAdminService)
        {
            _formAdminService = formAdminService;
        }

        public ViewResult Index(FormSearchModel model)
        {
            ViewData["results"] = _formAdminService.Search(model);

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
                return RedirectToAction("Edit", new { form.Id });
            }

            return RedirectToAction("Index");
        }

        public ViewResult Edit(int id)
        {
            var form = _formAdminService.GetForm(id);
            ViewData["form"] = form;
            return View(form);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(UpdateFormModel model)
        {
            await _formAdminService.Update(model);

            TempData.SuccessMessages().Add($"'{model.Name}' updated");

            return RedirectToAction("Edit", new { model.Id });
        }

        public ViewResult Delete(int id)
        {
            return View(_formAdminService.GetUpdateModel(id));
        }

        [HttpPost, ActionName(nameof(Delete))]
        public async Task<RedirectToActionResult> Delete_POST(int id)
        {
            await _formAdminService.Delete(id);
            return RedirectToAction("Index");
        }

        public PartialViewResult Postings(Form form, int page = 1, string search = null)
        {
            var data = _formAdminService.GetFormPostings(form, page, search);

            return PartialView(data);
        }

        public ActionResult ViewPosting(FormPosting formPosting)
        {
            return PartialView(formPosting);
        }

        [HttpGet]
        public async Task<ActionResult> Sort(int id)
        {

            return View(await _formAdminService.GetSortItems(id));
        }

        [HttpPost]
        public async Task Sort(List<SortItem> items)
        {
            await _formAdminService.SetOrders(items);
        }

        [HttpGet]
        public PartialViewResult ClearFormData(Form form)
        {
            return PartialView(form);
        }

        [HttpPost]
        [ActionName("ClearFormData")]
        public async Task<RedirectToActionResult> ClearFormData_POST(Form form)
        {
            await _formAdminService.ClearFormData(form);
            return RedirectToAction("Edit", "Form", new { id = form.Id });
        }

        [HttpGet]
        public async Task<ActionResult> ExportFormData(Form form)
        {
            try
            {
                var file = _formAdminService.ExportFormData(form);
                return File(await file, "text/csv", "MrCMS-FormData-[" + form.Name + "]-" + DateTime.UtcNow + ".csv");
            }
            catch
            {
                return RedirectToAction("Edit", "Form", new { id = form.Id });
            }
        }

        [HttpGet]
        public ViewResult DeleteEntry(FormPosting posting)
        {
            return View(posting);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteEntry(int id)
        {
            var posting = await _formAdminService.DeletePosting(id);
            return RedirectToAction("Edit", "Form", new { id = posting.Form.Id });
        }

        [HttpGet]
        public PartialViewResult FormProperties(int id)
        {
            return PartialView(_formAdminService.GetForm(id));
        }
    }
}