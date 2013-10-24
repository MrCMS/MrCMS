using System.Web.Mvc;
using MrCMS.Entities.Messaging;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MessageQueueController : MrCMSAdminController
    {
        private readonly IMessageQueueAdminService _messageQueueAdminService;

        public MessageQueueController(IMessageQueueAdminService messageQueueAdminService)
        {
            _messageQueueAdminService = messageQueueAdminService;
        }

        public ViewResult Index(MessageQueueQuery searchQuery)
        {
            ViewData["data"] = _messageQueueAdminService.GetMessages(searchQuery);
            return View(searchQuery);
        }

        public ActionResult Show(QueuedMessage queuedMessage)
        {
            return View(queuedMessage);
        }
    }
}