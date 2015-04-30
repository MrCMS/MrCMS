using System.Collections.Generic;
using System.IO;

namespace MrCMS.Services.ImportExport
{
    public interface IImportExportManager
    {
        byte[] ExportDocumentsToExcel();
        ImportDocumentsResult ImportDocumentsFromExcel(Stream file, bool autoStart = true);
    }
}