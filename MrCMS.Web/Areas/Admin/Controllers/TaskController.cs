using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
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
        public RedirectToRouteResult Edit(TaskUpdateData taskInfo)
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
        public RedirectToRouteResult Reset(TaskUpdateData taskInfo)
        {
            _taskAdminService.Reset(taskInfo);

            return RedirectToAction("Index");
        }
    }
}