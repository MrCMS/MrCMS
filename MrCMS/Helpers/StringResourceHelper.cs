using System.Web;
using System.Web.Mvc;
using MrCMS.Services.Resources;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Helpers
{
    public static class StringResourceHelper
    {
        /// <summary>
        /// Helper method to allow easier use of string resources
        /// </summary>
        /// <param name="key">The key. This is also the default value of the resource if the name is not specified</param>
        /// <param name="kernel">Kernel used to resolve the provider</param>
        /// <param name="defaultValue">Optionally specify the default value of the resource, if it differs from the key</param>
        /// <returns></returns>
        public static string AsResource(this string key, IKernel kernel, string defaultValue = null)
        {
            var provider = kernel.Get<IStringResourceProvider>();

            return GetValue(key, defaultValue, provider);
        }

        private static string GetValue(string key, string defaultValue, IStringResourceProvider provider)
        {
            return provider.GetValue(key, defaultValue);
        }

        /// <summary>
        /// Helper method to allow easier use of string resources
        /// </summary>
        /// <param name="key">The key. This is also the default value of the resource if the name is not specified</param>
        /// <param name="context">HttpContextBase used to resolve the provider</param>
        /// <param name="defaultValue">Optionally specify the default value of the resource, if it differs from the key</param>
        /// <returns></returns>
        public static string AsResource(this string key, HttpContextBase context, string defaultValue = null)
        {
            var provider = context.Get<IStringResourceProvider>();

            return GetValue(key, defaultValue, provider);
        }

        /// <summary>
        /// Helper method to allow easier use of string resources
        /// </summary>
        /// <param name="key">The key. This is also the default value of the resource if the name is not specified</param>
        /// <param name="helper">HtmlHelper used to resolve the provider</param>
        /// <param name="defaultValue">Optionally specify the default value of the resource, if it differs from the key</param>
        /// <returns></returns>
        public static string AsResource(this string key, HtmlHelper helper, string defaultValue = null)
        {
            var provider = helper.Get<IStringResourceProvider>();

            return GetValue(key, defaultValue, provider);
        }
    }
}