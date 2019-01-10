using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IGetWebpageCachingInfo
    {
        CachingInfo Get(Webpage webpage, object queryData = null);
    }
}