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
        /// <param name="value">The default value. This is also the name of the resouce if the name is not specified</param>
        /// <param name="kernel">Kernel used to resolve the provider</param>
        /// <param name="name">Optionally specify the name of the resource, if it is to differ from the value</param>
        /// <returns></returns>
        public static string AsResource(this string value, IKernel kernel, string name = null)
        {
            var provider = kernel.Get<IStringResourceProvider>();

            return GetValue(value, name, provider);
        }

        private static string GetValue(string value, string name, IStringResourceProvider provider)
        {
            return provider.GetValue(name ?? value, value);
        }

        /// <summary>
        /// Helper method to allow easier use of string resources
        /// </summary>
        /// <param name="value">The default value. This is also the name of the resouce if the name is not specified</param>
        /// <param name="context">HttpContextBase used to resolve the provider</param>
        /// <param name="name">Optionally specify the name of the resource, if it is to differ from the value</param>
        /// <returns></returns>
        public static string AsResource(this string value, HttpContextBase context, string name = null)
        {
            var provider = context.Get<IStringResourceProvider>();

            return GetValue(value, name, provider);
        }

        /// <summary>
        /// Helper method to allow easier use of string resources
        /// </summary>
        /// <param name="value">The default value. This is also the name of the resouce if the name is not specified</param>
        /// <param name="helper">HtmlHelper used to resolve the provider</param>
        /// <param name="name">Optionally specify the name of the resource, if it is to differ from the value</param>
        /// <returns></returns>
        public static string AsResource(this string value, HtmlHelper helper, string name = null)
        {
            var provider = helper.Get<IStringResourceProvider>();

            return GetValue(value, name, provider);
        }
    }
}