using System;
using System.Web.Mvc;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MessageTemplateController : MrCMSAdminController
    {
        private readonly IMessageTemplateService _messageTemplateService;

        public MessageTemplateController(IMessageTemplateService messageTemplateService)
        {
            _messageTemplateService = messageTemplateService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(_messageTemplateService.GetAllMessageTemplateTypesWithDetails());
        }

        [HttpGet]
        public ActionResult Add(string type)
        {
            var template = _messageTemplateService.GetNew(type);
            if (template != null)
            {
                ViewBag.AvailableTokens = template.GetTokens();
                return View(template);
            }
     
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Add")]
        public virtual ActionResult Add_POST([IoCModelBinder(typeof(AddMessageTemplateModelBinder))] MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                _messageTemplateService.Save(messageTemplate);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                ViewBag.AvailableTokens = messageTemplate.GetTokens();
                return View(messageTemplate);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Edit")]
        public virtual ActionResult Edit_POST([IoCModelBinder(typeof(EditMessageTemplateModelBinder))] MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                _messageTemplateService.Save(messageTemplate);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public virtual ActionResult Reset(MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                _messageTemplateService.Reset(messageTemplate);
                ViewBag.AvailableTokens = messageTemplate.GetTokens();

                return View("Edit", messageTemplate);
            }
            return RedirectToAction("Index");
        }
    }
}
