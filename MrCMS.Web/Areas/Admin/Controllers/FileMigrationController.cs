using System.Web.Mvc;
using MrCMS.Services.FileMigration;
using MrCMS.Web.Areas.Admin.Helpers;
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
            FileMigrationResult result = _fileMigrationService.MigrateFiles();

            if (result.MigrationRequired)
                TempData.SuccessMessages().Add(result.Message);
            else
                TempData.InfoMessages().Add(result.Message);

            return RedirectToAction("FileSystem", "Settings");
        }
    }
}