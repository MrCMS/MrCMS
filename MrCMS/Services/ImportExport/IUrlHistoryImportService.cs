using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport
{
    public interface IUrlHistoryImportService
    {
        List<UrlHistoryInfo> GetAllOtherUrls(Webpage webpage);
    }
}