using System.Collections.Generic;
using System.IO;

namespace MrCMS.Services.ImportExport
{
    public interface IImportExportManager
    {
        byte[] ExportDocumentsToExcel();
        Dictionary<string, List<string>> ImportDocumentsFromExcel(Stream file);
    }
}