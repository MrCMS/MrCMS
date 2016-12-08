using System.Threading;
using System.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using Ninject;

namespace MrCMS.Website
{
    public static class RequestAuthenticator
    {
        public static void Authenticate(HttpRequest request)
        {
            if (CurrentRequestData.CurrentContext.User != null)
            {
                var currentUser = MrCMSKernel.Kernel.Get<IUserLookup>()
                    .GetCurrentUser(CurrentRequestData.CurrentContext);
                if (!request.Url.AbsolutePath.StartsWith("/signalr/") && (currentUser == null ||
                                                                          !currentUser.IsActive))
                    MrCMSKernel.Kernel.Get<IAuthorisationService>().Logout();
                else
                {
                    CurrentRequestData.CurrentUser = currentUser;
                    Thread.CurrentThread.CurrentCulture = currentUser.GetUICulture();
                    Thread.CurrentThread.CurrentUICulture = currentUser.GetUICulture();
                }
            }
        }
    }
}