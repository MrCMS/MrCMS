using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Services.FileMigration;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Infrastructure.Helpers;

namespace MrCMS.Web.Admin.Controllers
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
        public async Task<ActionResult> Migrate()
        {
            FileMigrationResult result = await _fileMigrationService.MigrateFiles();

            if (result.MigrationRequired)
                TempData.AddSuccessMessage(result.Message);
            else
                TempData.AddInfoMessage(result.Message);

            return RedirectToAction("FileSystem", "Settings");
        }
    }
}