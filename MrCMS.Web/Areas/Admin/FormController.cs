using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin
{
    public class FormController : MrCMSAdminController
    {
        private readonly IDocumentService _documentService;
        private readonly IFormService _formService;

        public FormController(IDocumentService documentService, IFormService formService)
        {
            _documentService = documentService;
            _formService = formService;
        }

        [HttpGet]
        public ViewResult AddProperty(Webpage webpage)
        {
            return View(new TextBox { Webpage = webpage });
        }

        [HttpPost]
        public JsonResult AddProperty([IoCModelBinder(typeof(AddFormPropertyModelBinder))] FormProperty formProperty)
        {
            _formService.AddFormProperty(formProperty);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public ViewResult EditProperty(FormProperty property)
        {
            return View(property);
        }

        [HttpPost]
        [ActionName("EditProperty")]
        public JsonResult EditProperty_POST(FormProperty property)
        {
            _formService.SaveFormProperty(property);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public ViewResult DeleteProperty(FormProperty property)
        {
            return View(property);
        }
        [HttpPost]
        [ActionName("DeleteProperty")]
        public JsonResult DeleteProperty_POST(FormProperty property)
        {
            _formService.DeleteFormProperty(property);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public ActionResult AddOption(FormProperty formProperty)
        {
            return View(new FormListOption { FormProperty = formProperty });
        }

        [HttpPost]
        public ActionResult AddOption(FormListOption formListOption)
        {
            _formService.SaveFormListOption(formListOption);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public ActionResult EditOption(FormListOption formListOption)
        {
            return View(formListOption);
        }

        [HttpPost]
        [ActionName("EditOption")]
        public ActionResult EditOption_POST(FormListOption formListOption)
        {
            _formService.UpdateFormListOption(formListOption);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public ActionResult DeleteOption(FormListOption formListOption)
        {
            return View(formListOption);
        }

        [HttpPost]
        [ActionName("DeleteOption")]
        public ActionResult DeleteOption_POST(FormListOption formListOption)
        {
            _formService.DeleteFormListOption(formListOption);
            return Json(new FormActionResult { success = true });
        }
    }

    public class FormActionResult
    {
        public bool success { get; set; }
        public string message { get; set; }
    }
}