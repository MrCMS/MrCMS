using System.Web.Mvc;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Services;
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
            return View(TypeHelper.GetAllConcreteMappedClassesAssignableFrom<MessageTemplate>());
        }

        [HttpPost]
        public ActionResult Edit(MessageTemplate messageTemplate)
        {
            return RedirectToAction("Index");
        }
    }
}
