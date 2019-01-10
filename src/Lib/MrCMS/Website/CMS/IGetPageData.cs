using System.Threading.Tasks;

namespace MrCMS.Website.CMS
{
    public interface IGetPageData
    {
        PageData GetData(string url, string method);
    }
}