using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IUniquePageService
    {
        RedirectResult RedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage;
        T GetUniquePage<T>() where T : Document, IUniquePage;
    }
}