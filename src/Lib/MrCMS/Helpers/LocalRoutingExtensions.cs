using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Helpers
{
    public static class LocalRoutingExtensions
    {
        public static RedirectResult RedirectToLocal(this Controller controller, string url)
        {
            return controller.Redirect(
                controller.Url.IsLocalUrl(url)
                    ? url
                    : "~/"
            );
        }
    }
}