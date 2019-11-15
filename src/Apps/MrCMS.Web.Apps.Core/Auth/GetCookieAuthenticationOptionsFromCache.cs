using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;
using System;

namespace MrCMS.Web.Apps.Core.Auth
{
    public class GetCookieAuthenticationOptionsFromCache : IPostConfigureOptions<CookieAuthenticationOptions>, IOptionsMonitorCache<CookieAuthenticationOptions>
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public IUniquePageService PageService =>
            Context?.RequestServices.GetRequiredService<IUniquePageService>();

        public GetCookieAuthenticationOptionsFromCache(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public PathString AccessDeniedPath
            => PageService?.GetUrl<LoginPage>();
        public PathString LoginPath
            => PageService?.GetUrl<LoginPage>();

        public PathString LogoutPath => $"/{LogoutController.RouteUrl}";
        public void PostConfigure(string name, CookieAuthenticationOptions options)
        {
            options.AccessDeniedPath = AccessDeniedPath;
            options.LoginPath = LoginPath;
            options.LogoutPath = LogoutPath;
            options.ForwardDefaultSelector = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    return "Bearer";
                }
                else
                {
                    return CookieAuthenticationDefaults.AuthenticationScheme;
                }
            };
        }

        public CookieAuthenticationOptions GetOrAdd(string name, Func<CookieAuthenticationOptions> createOptions)
        {
            var key = GetKey(name);
            if (Context?.Items.ContainsKey(key) == true && Context.Items[key] is CookieAuthenticationOptions options)
            {
                return options;
            }

            options = createOptions();
            if (Context != null)
            {
                Context.Items[key] = options;
            }

            return options;
        }

        private static string GetKey(string name)
        {
            var key = Key + name;
            return key;
        }

        private const string Key = "current.cookie-authentication-options.";

        private HttpContext Context => _contextAccessor.HttpContext;

        public bool TryAdd(string name, CookieAuthenticationOptions options)
        {
            var key = GetKey(name);
            if (Context == null) return false;
            Context.Items[key] = options;
            return true;

        }

        public bool TryRemove(string name)
        {
            var key = GetKey(name);
            return Context != null && Context.Items.Remove(key);
        }

        public void Clear()
        {
        }
    }
}