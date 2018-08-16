using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class RedirectPageMiddleware : IMiddleware
    {
        private readonly IGetCurrentPage _getCurrentPage;

        public RedirectPageMiddleware(IGetCurrentPage getCurrentPage)
        {
            _getCurrentPage = getCurrentPage;
        }
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Webpage webpage = _getCurrentPage.GetPage();
            if (webpage is Redirect redirect)
            {
                string redirectUrl = redirect.RedirectUrl;
                // if the url is absolute, just redirect to it
                if (Uri.TryCreate(redirectUrl, UriKind.Absolute, out Uri result))
                {
                    context.Response.Redirect(redirectUrl, redirect.Permanent);
                }
                // otherwise sanitise it as a relative url and redirect
                else
                {
                    if (redirectUrl.StartsWith("/"))
                    {
                        redirectUrl = redirectUrl.Substring(1);
                    }

                    context.Response.Redirect("/" + redirectUrl, redirect.Permanent);
                }

                return Task.CompletedTask;
            }

            return next(context);
        }
    }
}