using System.Web;
using MrCMS.Services.Resources;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Helpers
{
    public static class StringResourceHelper
    {
        public static string AsResource(this string value, IKernel kernel)
        {
            var provider = kernel.Get<IStringResourceProvider>();

            return provider.GetValue(value);
        }
    }
}