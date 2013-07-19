using System;
using System.Web;
using System.Web.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Services.ImportExport;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class ImportExportController : MrCMSAdminController
    {
        private readonly IImportExportManager _importExportManager;

        public ImportExportController(IImportExportManager importExportManager)
        {
            _importExportManager = importExportManager;
        }

        [HttpGet]
        public ActionResult Documents()
        {
            return View();
        }

        [HttpGet]
        public FileResult ExportDocuments()
        {
            try
            {
                byte[] file = _importExportManager.ExportDocumentsToExcel();
                ViewBag.ExportStatus = "Documents successfully exported.";
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MrCMS-Documents-" + DateTime.UtcNow + ".xlsx");
            }
            catch (Exception)
            {
                ViewBag.ExportStatus = "Documents exporting has failed. Please try again and contact system administration if error continues to appear.";
                return null;
            }
        }

        [HttpPost]
        public ViewResult ImportDocuments(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0 && document.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                ViewBag.Messages = _importExportManager.ImportDocumentsFromExcel(document.InputStream);
            }
            else
            {
                ViewBag.ImportStatus = "Please choose non-empty Excel (.xslx) file before uploading.";
            }
            return View("Documents");
        }
    }
}
