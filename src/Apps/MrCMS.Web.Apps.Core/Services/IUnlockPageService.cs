using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Models;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IUnlockPageService
    {
        Webpage GetLockedPage(int id);
        UnlockPageResult TryUnlockPage(UnlockPageModel model, IResponseCookies cookies);
        RedirectResult RedirectToPage(int id);
        RedirectResult RedirectBackToPage(UnlockPageModel model);
    }
}