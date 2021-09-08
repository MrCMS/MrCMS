using System.IO;
using System.Threading.Tasks;
using MrCMS.Models;

namespace MrCMS.Services.ImportExport
{
    public interface IImportExportManager
    {
        Task<byte[]> ExportDocumentsToExcel();
        Task<ImportDocumentsResult> ImportDocumentsFromExcel(Stream file, bool autoStart = true);
        Task<ExportDocumentsResult> ExportDocumentsToEmail(ExportDocumentsModel model);
    }
}