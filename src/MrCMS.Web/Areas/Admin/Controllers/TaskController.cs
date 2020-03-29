using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Tasks;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class TaskController : MrCMSAdminController
    {
        private readonly ITaskAdminService _taskAdminService;

        public TaskController(ITaskAdminService taskAdminService)
        {
            _taskAdminService = taskAdminService;
        }

        public async Task<ViewResult> Index(QueuedTaskSearchQuery searchQuery)
        {
            ViewData["scheduled-tasks"] =await _taskAdminService.GetAllScheduledTasks();
            ViewData["tasks"] = await _taskAdminService.GetQueuedTasks(searchQuery);
            return View(searchQuery);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string type)
        {
            var taskInfo = await _taskAdminService.GetTaskUpdateData(type);
            return PartialView(taskInfo);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(TaskUpdateData taskInfo)
        {
            await _taskAdminService.Update(taskInfo);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<PartialViewResult> Reset(string type)
        {
            var taskInfo = await _taskAdminService.GetTaskUpdateData(type);
            return PartialView(taskInfo);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Reset(TaskUpdateData taskInfo)
        {
            await _taskAdminService.Reset(taskInfo);

            return RedirectToAction("Index");
        }
    }
}