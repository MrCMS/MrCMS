using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Routing
{
    public interface IGetErrorPage
    {
        Webpage GetPage(int code);
    }
}