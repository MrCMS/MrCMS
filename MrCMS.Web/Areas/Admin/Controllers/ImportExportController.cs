using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using MrCMS.Helpers.Validation;
using MrCMS.Models;
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
        public ActionResult Documents()
        {
            if (TempData.ContainsKey("messages"))
                ViewBag.Messages = TempData["messages"];
            if (TempData.ContainsKey("import-status"))
                ViewBag.ImportStatus = TempData["import-status"];
            if (TempData.ContainsKey("export-status"))
                ViewBag.ExportStatus = TempData["export-status"];
            return View();
        }

        [HttpGet]
        public ActionResult ExportDocuments()
        {
            try
            {
                byte[] file = _importExportManager.ExportDocumentsToExcel();
                TempData["export-status"] = "Documents successfully exported.";
                return File(file, ImportExportManager.XlsxContentType,
                    "MrCMS-Documents-" + DateTime.UtcNow + ".xlsx");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                TempData["export-status"] =
                    "Documents exporting has failed. Please try again and contact system administration if error continues to appear.";
                return RedirectToAction("Documents");
            }
        }

        [HttpPost]
        public ActionResult ImportDocuments(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0 &&
                document.ContentType == ImportExportManager.XlsxContentType)
                TempData["messages"] = _importExportManager.ImportDocumentsFromExcel(document.InputStream);
            else
                TempData["import-status"] = "Please choose non-empty Excel (.xslx) file before uploading.";
            return RedirectToAction("Documents");
        }

        [HttpPost]
        public ActionResult ExportDocumentsToEmail(ExportDocumentsModel model)
        {
            try
            {
                _importExportManager.ExportDocumentsToEmail(model);
                TempData["export-status"] = "Documents successfully exported.";
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                TempData["export-status"] =
                    "Documents exporting has failed. Please try again and contact system administration if error continues to appear.";
            }
            return RedirectToAction("Documents");
        }
    }
}
