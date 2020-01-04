using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Models;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IUnlockPageService
    {
        Task<Webpage> GetLockedPage(int id);
        Task<UnlockPageResult> TryUnlockPage(UnlockPageModel model, IResponseCookies cookies);
        Task<RedirectResult> RedirectToPage(int id);
        RedirectResult RedirectBackToPage(UnlockPageModel model);
    }
}