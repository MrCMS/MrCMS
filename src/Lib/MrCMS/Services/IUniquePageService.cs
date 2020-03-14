using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IUniquePageService
    {
        Task<RedirectResult> RedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage;
        Task<RedirectResult> PermanentRedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage;
        Task<T> GetUniquePage<T>() where T : Webpage, IUniquePage;
        Task<string> GetUrl<T>(object queryString = null) where T : Webpage, IUniquePage;
    }
}