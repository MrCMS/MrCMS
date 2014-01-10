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
            return PartialView(_fileMigrationService.FilesToMigrate());
        }

        public ActionResult Migrate(int number = 100)
        {
            _fileMigrationService.MigrateFilesToAzure(number);
            return RedirectToAction("FileSystem", "Settings");
        }
    }
}