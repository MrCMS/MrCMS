using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface IGetErrorPage
    {
        Webpage GetPage(int code);
    }
}