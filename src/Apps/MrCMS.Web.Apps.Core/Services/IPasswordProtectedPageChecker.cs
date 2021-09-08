using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IPasswordProtectedPageChecker
    {
        Task<bool> CanAccessPage(Webpage webpage, IRequestCookieCollection cookies);
        void GiveAccessToPage(Webpage webpage, IResponseCookies cookies);
    }
}