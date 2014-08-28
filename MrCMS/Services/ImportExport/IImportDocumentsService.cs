using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Website;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Linq;

namespace MrCMS.Services.ImportExport
{
    public interface IImportDocumentsService
    {
        void ImportDocumentsFromDTOs(IEnumerable<DocumentImportDTO> items);
        Webpage ImportDocument(DocumentImportDTO dto);
    }

    public interface IExportDocumentsService
    {
        ExcelPackage GetExportExcelPackage(List<Webpage> webpages);
        byte[] ConvertPackageToByteArray(ExcelPackage package);
    }

    public class ExportDocumentsService : IExportDocumentsService
    {
        public ExcelPackage GetExportExcelPackage(List<Webpage> webpages)
        {
            var excelFile = new ExcelPackage();

            CreateInfoSheet(excelFile);

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


            for (var i = 0; i < webpages.Count; i++)
            {
                AddWebpage(webpages, i, wsItems);
            }
            wsItems.Cells["A:D"].AutoFitColumns();
            wsItems.Cells["F:F"].AutoFitColumns();
            wsItems.Cells["H:N"].AutoFitColumns();

            return excelFile;

        }

        private static void CreateInfoSheet(ExcelPackage excelFile)
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
        }

        private static void AddWebpage(List<Webpage> webpages, int index, ExcelWorksheet wsItems)
        {
            var rowId = index + 2;
            var webpage = webpages[index];
            wsItems.Cells["A" + rowId].Value = webpage.UrlSegment;
            wsItems.Cells["A" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            wsItems.Cells["B" + rowId].Value = webpage.Parent != null ? webpage.Parent.UrlSegment : String.Empty;
            wsItems.Cells["B" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            wsItems.Cells["C" + rowId].Value = webpage.DocumentType;
            wsItems.Cells["D" + rowId].Value = webpage.Name;
            wsItems.Cells["D" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            wsItems.Cells["E" + rowId].Value = webpage.BodyContent;
            wsItems.Cells["E" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
            wsItems.Cells["F" + rowId].Value = webpage.MetaTitle;
            wsItems.Cells["G" + rowId].Value = webpage.MetaDescription;
            wsItems.Cells["G" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
            wsItems.Cells["H" + rowId].Value = webpage.MetaKeywords;
            wsItems.Cells["I" + rowId].Value = string.Join(",", webpage.Tags.Select(tag => tag.Name));
            wsItems.Cells["I" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            wsItems.Cells["J" + rowId].Value = webpage.RevealInNavigation;
            wsItems.Cells["K" + rowId].Value = webpage.DisplayOrder;
            wsItems.Cells["L" + rowId].Value = webpage.RequiresSSL;
            wsItems.Cells["M" + rowId].Value = webpage.PublishOn.HasValue
                ? webpage.PublishOn.Value.ToString("yyyy-MM-dd HH:mm:ss")
                : String.Empty;
            wsItems.Cells["N" + rowId].Value = string.Join(",", webpage.Urls.Select(history => history.UrlSegment));
            wsItems.Cells["N" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
        }

        public byte[] ConvertPackageToByteArray(ExcelPackage package)
        {
            return package.GetAsByteArray();
        }
    }
}