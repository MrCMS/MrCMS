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
            if (!String.IsNullOrWhiteSpace(type))
            {
                var newType = TypeHelper.GetTypeByClassName(type);
                if (newType != null)
                {
                    var messageTemplate = Activator.CreateInstance(newType) as MessageTemplate;
                    if (messageTemplate != null && messageTemplate as IMessageTemplate != null)
                        ViewBag.AvailableTokens = (messageTemplate as IMessageTemplate).GetTokens();
                    if (messageTemplate != null) return View(messageTemplate.GetInitialTemplate());
                }
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
                ViewBag.AvailableTokens = (messageTemplate as IMessageTemplate).GetTokens();
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
                messageTemplate.FromAddress = messageTemplate.GetInitialTemplate().FromAddress;
                messageTemplate.FromName = messageTemplate.GetInitialTemplate().FromName;
                messageTemplate.ToAddress = messageTemplate.GetInitialTemplate().ToAddress;
                messageTemplate.ToName = messageTemplate.GetInitialTemplate().ToName;
                messageTemplate.Bcc = messageTemplate.GetInitialTemplate().Bcc;
                messageTemplate.Cc = messageTemplate.GetInitialTemplate().Cc;
                messageTemplate.Subject = messageTemplate.GetInitialTemplate().Subject;
                messageTemplate.Body = messageTemplate.GetInitialTemplate().Body;
                messageTemplate.IsHtml = messageTemplate.GetInitialTemplate().IsHtml;

                _messageTemplateService.Save(messageTemplate);

                ViewBag.AvailableTokens = (messageTemplate as IMessageTemplate).GetTokens();

                return View("Edit", messageTemplate);
            }
            return RedirectToAction("Index");
        }
    }
}
