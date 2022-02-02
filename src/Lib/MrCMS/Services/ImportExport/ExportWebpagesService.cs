using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport
{
    public class ExportWebpagesService : IExportWebpagesService
    {
        public XLWorkbook GetExportExcelPackage(List<Webpage> webpages)
        {
            var excelFile = new XLWorkbook();

            CreateInfoSheet(excelFile);

            var wsItems = excelFile.Worksheets.Add("Items");

            wsItems.Cells("A1:N1").Style.Font.Bold = true;
            // wsItems.Cells("A:N").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            wsItems.Cells("A1").Value = "Url (Must not be changed!)";
            wsItems.Cells("B1").Value = "Parent Url";
            wsItems.Cells("C1").Value = "Webpage Type";
            wsItems.Cells("D1").Value = "Name";
            wsItems.Cells("E1").Value = "Body Content";
            wsItems.Cells("F1").Value = "SEO Title";
            wsItems.Cells("G1").Value = "SEO Description";
            wsItems.Cells("H1").Value = "SEO Keywords";
            wsItems.Cells("I1").Value = "Tags";
            wsItems.Cells("J1").Value = "Reveal in navigation";
            wsItems.Cells("K1").Value = "Display Order";
            wsItems.Cells("L1").Value = "Publish Date";
            wsItems.Cells("M1").Value = "Url History";


            for (var i = 0; i < webpages.Count; i++)
            {
                AddWebpage(webpages, i, wsItems);
            }

            // wsItems.Columns("A:D").AdjustToContents();
            // wsItems.Columns("F:F").AdjustToContents();
            // wsItems.Columns("H:N").AdjustToContents();

            return excelFile;
        }

        public byte[] ConvertPackageToByteArray(XLWorkbook package)
        {
            using var stream = new MemoryStream();
            package.SaveAs(stream);
            return stream.ToArray();
        }

        private static void CreateInfoSheet(XLWorkbook excelFile)
        {
            var wsInfo = excelFile.Worksheets.Add("Info");

            wsInfo.Cells("A1:D1").Style.Font.Bold = true;
            // wsInfo.Cells("A:D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            wsInfo.Cells("A1").Value = "MrCMS Version";
            wsInfo.Cells("B1").Value = "Entity Type for Export";
            wsInfo.Cells("C1").Value = "Export Date";
            wsInfo.Cells("D1").Value = "Export Source";

            var assemblyVersion = typeof(ExportWebpagesService).Assembly.GetCustomAttribute<AssemblyVersionAttribute>()
                ?.Version;
            wsInfo.Cells("A2").Value = assemblyVersion;
            wsInfo.Cells("B2").Value = "Webpage";
            wsInfo.Cells("C2").Style.NumberFormat.Format = "YYYY-MM-DDThh:mm:ss.sTZD";
            wsInfo.Cells("C2").Value = DateTime.UtcNow;
            wsInfo.Cells("D2").Value = $"MrCMS {assemblyVersion}";

            wsInfo.Columns("A:D").AdjustToContents();
            wsInfo.Cells("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            wsInfo.Cells("A4").Value = "Please do not change any values inside this file.";
        }

        private static void AddWebpage(List<Webpage> webpages, int index, IXLWorksheet wsItems)
        {
            var rowId = index + 2;
            var webpage = webpages[index];
            wsItems.Cells("A" + rowId).Value = webpage.UrlSegment;
            wsItems.Cells("A" + rowId).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            wsItems.Cells("B" + rowId).Value = webpage.Parent != null ? webpage.Parent.UrlSegment : String.Empty;
            wsItems.Cells("B" + rowId).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            wsItems.Cells("C" + rowId).Value = webpage.WebpageType;
            wsItems.Cells("D" + rowId).Value = webpage.Name;
            wsItems.Cells("D" + rowId).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            wsItems.Cells("E" + rowId).Value = webpage.BodyContent;
            wsItems.Cells("E" + rowId).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Fill;
            wsItems.Cells("F" + rowId).Value = webpage.MetaTitle;
            wsItems.Cells("G" + rowId).Value = webpage.MetaDescription;
            wsItems.Cells("G" + rowId).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Fill;
            wsItems.Cells("H" + rowId).Value = webpage.MetaKeywords;
            wsItems.Cells("I" + rowId).Value = string.Join(",", webpage.Tags.Select(tag => tag.Name));
            wsItems.Cells("I" + rowId).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            wsItems.Cells("J" + rowId).Value = webpage.RevealInNavigation;
            wsItems.Cells("K" + rowId).Value = webpage.DisplayOrder;
            wsItems.Cells("L" + rowId).Value = webpage.PublishOn.HasValue
                ? webpage.PublishOn.Value.ToString("yyyy-MM-dd HH:mm:ss")
                : String.Empty;
            wsItems.Cells("M" + rowId).Value = string.Join(",", webpage.Urls.Select(history => history.UrlSegment));
            wsItems.Cells("M" + rowId).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        }
    }
}