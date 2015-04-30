using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MessageTemplatePreviewController : MrCMSAdminController
    {
        private readonly IMessageTemplatePreviewService _messageTemplatePreviewService;

        public MessageTemplatePreviewController(IMessageTemplatePreviewService messageTemplatePreviewService)
        {
            _messageTemplatePreviewService = messageTemplatePreviewService;
        }

        public ViewResult Get(string type)
        {
            return View(_messageTemplatePreviewService.GetTemplate(type));
        }

        public PartialViewResult Preview(string type, int id)
        {
            return PartialView(_messageTemplatePreviewService.GetPreview(type, id));
        }
    }
}