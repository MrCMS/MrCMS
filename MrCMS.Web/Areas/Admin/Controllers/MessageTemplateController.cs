using System.Web.Mvc;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MessageTemplateController : MrCMSAdminController
    {
        private readonly IMessageTemplateAdminService _messageTemplateAdminService;

        public MessageTemplateController(IMessageTemplateAdminService messageTemplateAdminService)
        {
            _messageTemplateAdminService = messageTemplateAdminService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(_messageTemplateAdminService.GetAllMessageTemplateTypesWithDetails());
        }

        [HttpGet]
        public ActionResult Add(string type)
        {
            var template = _messageTemplateAdminService.GetNew(type);
            if (template != null)
            {
                return View(template);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Add")]
        public ActionResult Add_POST([IoCModelBinder(typeof(AddMessageTemplateModelBinder))] MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                _messageTemplateAdminService.Save(messageTemplate);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(MessageTemplate messageTemplate)
        {
            ModelState.Clear();
            if (messageTemplate != null)
            {
                return View(messageTemplate);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Edit")]
        public ActionResult Edit_POST(MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                _messageTemplateAdminService.Save(messageTemplate);
                TempData.SuccessMessages().Add(string.Format("{0} successfully edited", messageTemplate.MessageTemplateType.BreakUpString()));
                return RedirectToAction("Edit", new { id = messageTemplate.Id });
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Reset(MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                return PartialView(messageTemplate);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Reset")]
        public RedirectToRouteResult Reset_POST(MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                _messageTemplateAdminService.Reset(messageTemplate);
                return RedirectToAction("Edit", new { id = messageTemplate.Id });
            }
            return RedirectToAction("Index");
        }

        public PartialViewResult Tokens(MessageTemplate messageTemplate)
        {
            return PartialView(_messageTemplateAdminService.GetTokens(messageTemplate));
        }

        public ActionResult Preview(MessageTemplate messageTemplate)
        {
            return PartialView(messageTemplate);
        }

        public string GetPreview(MessageTemplate messageTemplate, int itemId)
        {
            return _messageTemplateAdminService.GetPreview(messageTemplate, itemId);
        }
    }
}
