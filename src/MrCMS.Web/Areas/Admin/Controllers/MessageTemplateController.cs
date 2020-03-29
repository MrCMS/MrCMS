using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Helpers;
using MrCMS.Messages;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Services;
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
        public async Task<ActionResult> Index()
        {
            return View(await _messageTemplateAdminService.GetAllMessageTemplateTypesWithDetails());
        }

        [HttpGet]
        public async Task<ActionResult> AddSiteOverride(string type)
        {
            return View(await _messageTemplateAdminService.GetNewOverride(type));
        }

        [HttpPost]
        [ActionName("AddSiteOverride")]
        public async Task<ActionResult> AddSiteOverride_POST(
            [ModelBinder(typeof(MessageTemplateOverrideModelBinder))]
            MessageTemplate messageTemplate)

        {
            if (messageTemplate != null)
            {
                await _messageTemplateAdminService.AddOverride(messageTemplate);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> DeleteSiteOverride(string type)
        {
            return View(await _messageTemplateAdminService.GetOverride(type));
        }

        [HttpPost]
        [ActionName("DeleteSiteOverride")]
        public async Task<ActionResult> DeleteSiteOverride_POST(string type)
        {
            if (type != null)
            {
                await _messageTemplateAdminService.DeleteOverride(type);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string type)
        {
            ModelState.Clear();
            MessageTemplate template = await _messageTemplateAdminService.GetTemplate(type);
            if (template != null)
            {
                return View(template);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<ActionResult> Edit_POST([ModelBinder(typeof(MessageTemplateOverrideModelBinder))] MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                await _messageTemplateAdminService.Save(messageTemplate);
                TempData.SuccessMessages()
                    .Add(string.Format("{0} successfully edited", messageTemplate.GetType().Name.BreakUpString()));
                return RedirectToAction("Edit", new { type = messageTemplate.GetType().FullName });
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ImportLegacyTemplate(string type)
        {
            return View((object)type);
        }

        [HttpPost, ActionName("ImportLegacyTemplate")]
        public async Task<ActionResult> ImportLegacyTemplate_POST(string type)
        {
            await _messageTemplateAdminService.ImportLegacyTemplate(type);
            return RedirectToAction("Index");
        }
    }
}