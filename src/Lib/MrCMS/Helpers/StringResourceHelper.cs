using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services.Resources;

namespace MrCMS.Helpers
{
    public static class StringResourceHelper
    {
        /// <summary>
        ///     Helper method to allow easier use of string resources
        /// </summary>
        /// <param name="key">The key. This is also the default value of the resource if the name is not specified</param>
        /// <param name="serviceProvider">Kernel used to resolve the provider</param>
        /// <param name="defaultValue">Optionally specify the default value of the resource, if it differs from the key</param>
        /// <returns></returns>
        public static async Task<string> AsResource(this string key, IServiceProvider serviceProvider, string defaultValue = null)
        {
            var provider = serviceProvider.GetRequiredService<IStringResourceProvider>();

            return await GetValue(key, defaultValue, provider);
        }

        private static async Task<string> GetValue(string key, string defaultValue, IStringResourceProvider provider)
        {
            return await provider.GetValue(key, defaultValue);
        }

        /// <summary>
        ///     Helper method to allow easier use of string resources
        /// </summary>
        /// <param name="key">The key. This is also the default value of the resource if the name is not specified</param>
        /// <param name="context">HttpContextBase used to resolve the provider</param>
        /// <param name="defaultValue">Optionally specify the default value of the resource, if it differs from the key</param>
        /// <returns></returns>
        public static async Task<string> AsResource(this string key, HttpContext context, string defaultValue = null)
        {
            var provider = context.RequestServices.GetRequiredService<IStringResourceProvider>();

            return await GetValue(key, defaultValue, provider);
        }

        /// <summary>
        ///     Helper method to allow easier use of string resources
        /// </summary>
        /// <param name="key">The key. This is also the default value of the resource if the name is not specified</param>
        /// <param name="helper">HtmlHelper used to resolve the provider</param>
        /// <param name="defaultValue">Optionally specify the default value of the resource, if it differs from the key</param>
        /// <returns></returns>
        public static async Task<string> AsResource(this string key, IHtmlHelper helper, string defaultValue = null)
        {
            var provider = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IStringResourceProvider>();

            return await GetValue(key, defaultValue, provider);
        }
    }
}