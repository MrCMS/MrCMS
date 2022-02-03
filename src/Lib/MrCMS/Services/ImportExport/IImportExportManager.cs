using System.IO;
using System.Threading.Tasks;
using MrCMS.Models;

namespace MrCMS.Services.ImportExport
{
    public interface IImportExportManager
    {
        Task<byte[]> ExportWebpagesToExcel();
        Task<ImportWebpagesResult> ImportWebpagesFromExcel(int siteId, Stream file, bool autoStart = true);
        Task<ExportWebpagesResult> ExportWebpagesToEmail(ExportWebpagesModel model);
    }
}