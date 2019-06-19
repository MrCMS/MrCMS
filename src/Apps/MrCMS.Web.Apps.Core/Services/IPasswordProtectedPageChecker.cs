using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IPasswordProtectedPageChecker
    {
        bool CanAccessPage(Webpage webpage, IRequestCookieCollection cookies);
        void GiveAccessToPage(Webpage webpage, IResponseCookies cookies);
    }
}