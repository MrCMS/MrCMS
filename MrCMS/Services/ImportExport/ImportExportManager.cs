using System.IO;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Website;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport
{
    public class ImportExportManager : IImportExportManager
    {
        private readonly IImportDocumentsValidationService _importDocumentsValidationService;
        private readonly IImportDocumentsService _importDocumentService;
        private readonly IDocumentService _documentService;

        public ImportExportManager(IImportDocumentsValidationService importDocumentsValidationService,
            IImportDocumentsService importDocumentsService, IDocumentService documentService)
        {
            _importDocumentsValidationService = importDocumentsValidationService;
            _importDocumentService = importDocumentsService;
            _documentService = documentService;
        }

        #region Import Documents
        public Dictionary<string, List<string>> ImportDocumentsFromExcel(Stream file)
        {
            var spreadsheet = new ExcelPackage(file);

            Dictionary<string, List<string>> parseErrors;
            var items = GetDocumentsFromSpreadSheet(spreadsheet, out parseErrors);
            if (parseErrors.Any())
                return parseErrors;
            var businessLogicErrors = _importDocumentsValidationService.ValidateBusinessLogic(items);
            if (businessLogicErrors.Any())
                return businessLogicErrors;
            _importDocumentService.ImportDocumentsFromDTOs(items);
            return new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Try and get data out of the spreadsheet into the DTOs with parse and type checks
        /// </summary>
        /// <param name="spreadsheet"></param>
        /// <param name="parseErrors"></param>
        /// <returns></returns>
        private List<DocumentImportDataTransferObject> GetDocumentsFromSpreadSheet(ExcelPackage spreadsheet, out Dictionary<string, List<string>> parseErrors)
        {
            parseErrors = _importDocumentsValidationService.ValidateImportFile(spreadsheet);

            return _importDocumentsValidationService.ValidateAndImportDocuments(spreadsheet, ref parseErrors);
        }

        #endregion

        #region Export Documents
        public byte[] ExportDocumentsToExcel()
        {
            using (var excelFile = new ExcelPackage())
            {
                var wsInfo = excelFile.Workbook.Worksheets.Add("Info");

                wsInfo.Cells["A1:D1"].Style.Font.Bold = true;
                wsInfo.Cells["A:D"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsInfo.Cells["A1"].Value = "MrCMS Version";
                wsInfo.Cells["B1"].Value = "Entity Type for Export";
                wsInfo.Cells["C1"].Value = "Export Date";
                wsInfo.Cells["D1"].Value = "Export Source";

                wsInfo.Cells["A2"].Value = MrCMSApplication.AssemblyVersion;
                wsInfo.Cells["B2"].Value = "Webpage";
                wsInfo.Cells["C2"].Style.Numberformat.Format = "YYYY-MM-DDThh:mm:ss.sTZD";
                wsInfo.Cells["C2"].Value = DateTime.UtcNow;
                wsInfo.Cells["D2"].Value = string.Format("MrCMS {0}", MrCMSApplication.AssemblyVersion);

                wsInfo.Cells["A:D"].AutoFitColumns();
                wsInfo.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                wsInfo.Cells["A4"].Value = "Please do not change any values inside this file.";

                var wsItems = excelFile.Workbook.Worksheets.Add("Items");

                wsItems.Cells["A1:N1"].Style.Font.Bold = true;
                wsItems.Cells["A:N"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsItems.Cells["A1"].Value = "Url (Must not be changed!)";
                wsItems.Cells["B1"].Value = "Parent Url";
                wsItems.Cells["C1"].Value = "Document Type";
                wsItems.Cells["D1"].Value = "Name";
                wsItems.Cells["E1"].Value = "Body Content";
                wsItems.Cells["F1"].Value = "SEO Title";
                wsItems.Cells["G1"].Value = "SEO Description";
                wsItems.Cells["H1"].Value = "SEO Keywords";
                wsItems.Cells["I1"].Value = "Tags";
                wsItems.Cells["J1"].Value = "Reveal in navigation";
                wsItems.Cells["K1"].Value = "Display Order";
                wsItems.Cells["L1"].Value = "Require SSL";
                wsItems.Cells["M1"].Value = "Publish Date";
                wsItems.Cells["N1"].Value = "Url History";

                var items = _documentService.GetAllDocuments<Webpage>().ToList();

                for (var i = 0; i < items.Count; i++)
                {
                    var rowId = i + 2;
                    wsItems.Cells["A" + rowId].Value = items[i].UrlSegment;
                    wsItems.Cells["A" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["B" + rowId].Value = items[i].Parent!=null?items[i].Parent.UrlSegment:String.Empty;
                    wsItems.Cells["B" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["C" + rowId].Value = items[i].DocumentType;
                    wsItems.Cells["D" + rowId].Value = items[i].Name;
                    wsItems.Cells["D" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["E" + rowId].Value = items[i].BodyContent;
                    wsItems.Cells["F" + rowId].Value = items[i].MetaTitle;
                    wsItems.Cells["G" + rowId].Value = items[i].MetaDescription;
                    wsItems.Cells["G" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
                    wsItems.Cells["H" + rowId].Value = items[i].MetaKeywords;
                    for (var j = 0; j < items[i].Tags.Count; j++)
                    {
                        wsItems.Cells["I" + rowId].Value += items[i].Tags[j].Name;
                        if (j != items[i].Tags.Count - 1)
                            wsItems.Cells["I" + rowId].Value += ",";
                    }
                    wsItems.Cells["I" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["J" + rowId].Value = items[i].RevealInNavigation;
                    wsItems.Cells["K" + rowId].Value = items[i].DisplayOrder;
                    wsItems.Cells["L" + rowId].Value = items[i].RequiresSSL;
                    wsItems.Cells["M" + rowId].Value = items[i].PublishOn.HasValue ? items[i].PublishOn.Value.ToString("yyyy-MM-dd HH:mm:ss") : String.Empty;
                    for (var j = 0; j < items[i].Urls.Count; j++)
                    {
                        wsItems.Cells["N" + rowId].Value += items[i].Urls[j].UrlSegment;
                        if (j != items[i].Urls.Count - 1)
                            wsItems.Cells["N" + rowId].Value += ",";
                    }
                    wsItems.Cells["N" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                }
                wsItems.Cells["A:D"].AutoFitColumns();
                wsItems.Cells["F:F"].AutoFitColumns();
                wsItems.Cells["H:N"].AutoFitColumns();

                return excelFile.GetAsByteArray();
            }
        }
        #endregion
    }
}