using System.Web.Mvc;
using MrCMS.Tasks;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class TaskController : MrCMSAdminController
    {
        private readonly IScheduledTaskManager _scheduledTaskManager;

        public TaskController(IScheduledTaskManager scheduledTaskManager)
        {
            _scheduledTaskManager = scheduledTaskManager;
        }

        public ViewResult Index()
        {
            return View(_scheduledTaskManager.GetAllTasks());
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView(new ScheduledTask());
        }

        [HttpPost]
        public RedirectToRouteResult Add(ScheduledTask scheduledTask)
        {
            _scheduledTaskManager.Add(scheduledTask);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(ScheduledTask scheduledTask)
        {
            return PartialView(scheduledTask);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_Post(ScheduledTask scheduledTask)
        {
            _scheduledTaskManager.Update(scheduledTask);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(ScheduledTask scheduledTask)
        {
            return PartialView(scheduledTask);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_Post(ScheduledTask scheduledTask)
        {
            _scheduledTaskManager.Delete(scheduledTask);
            return RedirectToAction("Index");
        }
    }
}