using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface IGetWebpageForPath
    {
        Webpage GetWebpage(string path);
    }
}