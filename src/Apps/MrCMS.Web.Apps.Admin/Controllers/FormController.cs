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
        public RedirectToActionResult Add(AddFormModel model)
        {
            var form = _formAdminService.AddForm(model);
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
        public RedirectToActionResult Edit(UpdateFormModel model)
        {
            _formAdminService.Update(model);

            return RedirectToAction("Edit", new {model.Id});
        }

        public ViewResult Delete(int id)
        {
            return View(_formAdminService.GetUpdateModel(id));
        }

        [HttpPost, ActionName(nameof(Delete))]
        public RedirectToActionResult Delete_POST(int id)
        {
            _formAdminService.Delete(id);
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
        public ActionResult Sort(Form form)
        {
            var sortItems = form.FormProperties.OrderBy(x => x.DisplayOrder)
                                .Select(
                                    arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
                                .ToList();

            return View(sortItems);
        }

        [HttpPost]
        public void Sort(List<SortItem> items)
        {
            _formAdminService.SetOrders(items);
        }

        [HttpGet]
        public PartialViewResult ClearFormData(Form form)
        {
            return PartialView(form);
        }

        [HttpPost]
        [ActionName("ClearFormData")]
        public RedirectToActionResult ClearFormData_POST(Form form)
        {
            _formAdminService.ClearFormData(form);
            return RedirectToAction("Edit", "Form", new { id = form.Id });
        }

        [HttpGet]
        public ActionResult ExportFormData(Form form)
        {
            try
            {
                var file = _formAdminService.ExportFormData(form);
                return File(file, "text/csv", "MrCMS-FormData-[" + form.Name + "]-" + DateTime.UtcNow + ".csv");
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
        public ActionResult DeleteEntry(int id)
        {
            var posting = _formAdminService.DeletePosting(id);
            return RedirectToAction("Edit", "Form", new { id = posting.Form.Id });
        }

        [HttpGet]
        public PartialViewResult FormProperties(int id)
        {
            return PartialView(_formAdminService.GetForm(id));
        }
    }
}