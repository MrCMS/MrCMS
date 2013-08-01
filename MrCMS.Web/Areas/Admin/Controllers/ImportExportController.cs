using System;
using System.Web;
using System.Web.Mvc;
using MrCMS.Website;
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
        public ActionResult Documents(string status)
        {
            ViewBag.ExportStatus = status;
            return View();
        }

        [HttpGet]
        public ActionResult ExportDocuments()
        {
            try
            {
                byte[] file = _importExportManager.ExportDocumentsToExcel();
                ViewBag.ExportStatus = "Documents successfully exported.";
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MrCMS-Documents-" + DateTime.UtcNow + ".xlsx");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                const string msg = "Documents exporting has failed. Please try again and contact system administration if error continues to appear.";
                return RedirectToAction("Documents", new { status = msg });
            }
        }

        [HttpPost]
        public ViewResult ImportDocuments(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0 && document.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                ViewBag.Messages = _importExportManager.ImportDocumentsFromExcel(document.InputStream);
            else
                ViewBag.ImportStatus = "Please choose non-empty Excel (.xslx) file before uploading.";
            return View("Documents");
        }
    }
}
