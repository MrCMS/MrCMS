using System.Web.Mvc;
using MrCMS.Services.FileMigration;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class FileMigrationController : MrCMSAdminController
    {
        private readonly IFileMigrationService _fileMigrationService;

        public FileMigrationController(IFileMigrationService fileMigrationService)
        {
            _fileMigrationService = fileMigrationService;
        }

        public PartialViewResult Show()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Migrate()
        {
            _fileMigrationService.MigrateFiles();
            return RedirectToAction("FileSystem", "Settings");
        }
    }
}