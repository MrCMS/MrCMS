using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using OfficeOpenXml;

namespace MrCMS.Services.ImportExport
{
    public interface IExportDocumentsService
    {
        ExcelPackage GetExportExcelPackage(List<Webpage> webpages);
        byte[] ConvertPackageToByteArray(ExcelPackage package);
    }
}