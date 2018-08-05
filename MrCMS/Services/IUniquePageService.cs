using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IUniquePageService
    {
        RedirectResult RedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage;
        RedirectResult PermanentRedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage;
        T GetUniquePage<T>() where T : Webpage, IUniquePage;
        string GetUrl<T>(object queryString = null) where T : Webpage, IUniquePage;
    }
}