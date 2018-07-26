using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using MrCMS.Services.ImportExport;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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
                //CurrentRequestData.ErrorSignal.Raise(ex);
                // TODO: logging
                TempData["export-status"] =
                    "Documents exporting has failed. Please try again and contact system administration if error continues to appear.";
                return RedirectToAction("Documents");
            }
        }

        [HttpPost]
        public ActionResult ImportDocuments(IFormFile document)
        {
            if (document != null && document.Length > 0 &&
                document.ContentType == ImportExportManager.XlsxContentType)
                TempData["messages"] = _importExportManager.ImportDocumentsFromExcel(document.OpenReadStream());
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
                //CurrentRequestData.ErrorSignal.Raise(ex);
                // TODO: logging
                TempData["export-status"] =
                    "Documents exporting has failed. Please try again and contact system administration if error continues to appear.";
            }
            return RedirectToAction("Documents");
        }
    }
}
