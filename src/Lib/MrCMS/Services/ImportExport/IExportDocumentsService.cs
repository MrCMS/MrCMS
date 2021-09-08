using System.Collections.Generic;
using ClosedXML.Excel;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport
{
    public interface IExportDocumentsService
    {
        XLWorkbook GetExportExcelPackage(List<Webpage> webpages);
        byte[] ConvertPackageToByteArray(XLWorkbook package);
    }
}