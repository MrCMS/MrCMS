using Microsoft.AspNetCore.Mvc;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class TaskController : MrCMSAdminController
    {
        private readonly ITaskAdminService _taskAdminService;

        public TaskController(ITaskAdminService taskAdminService)
        {
            _taskAdminService = taskAdminService;
        }

        public ViewResult Index(QueuedTaskSearchQuery searchQuery)
        {
            ViewData["scheduled-tasks"] = _taskAdminService.GetAllScheduledTasks();
            ViewData["tasks"] = _taskAdminService.GetQueuedTasks(searchQuery);
            return View(searchQuery);
        }

        [HttpGet]
        public PartialViewResult Edit(string type)
        {
            var taskInfo = _taskAdminService.GetTaskUpdateData(type);
            return PartialView(taskInfo);
        }

        [HttpPost]
        public RedirectToActionResult Edit(TaskUpdateData taskInfo)
        {
            _taskAdminService.Update(taskInfo);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Reset(string type)
        {
            var taskInfo = _taskAdminService.GetTaskUpdateData(type);
            return PartialView(taskInfo);
        }

        [HttpPost]
        public RedirectToActionResult Reset(TaskUpdateData taskInfo)
        {
            _taskAdminService.Reset(taskInfo);

            return RedirectToAction("Index");
        }
    }
}