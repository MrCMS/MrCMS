using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public interface IUpdateUrlHistoryService
    {
        Task SetUrlHistory(WebpageImportDTO webpageDto, Webpage webpage);
    }
}