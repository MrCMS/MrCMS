using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport
{
    public interface IUrlHistoryImportService
    {
        Task<List<UrlHistoryInfo>> GetAllOtherUrls(Webpage webpage);
    }
}