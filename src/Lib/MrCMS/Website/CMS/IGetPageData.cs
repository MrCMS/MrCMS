using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.CMS
{
    public interface IGetPageData
    {
        PageData GetData(string url, string method);
        PageData GetData(Webpage webpage, string method);
    }
}