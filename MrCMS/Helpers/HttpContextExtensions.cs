using System.Web;
using Ninject;

namespace MrCMS.Helpers
{
    public static class HttpContextExtensions
    {
        public const string KernelKey = "current.request.kernel";

        public static IKernel GetKernel(this HttpContextBase context)
        {
            return context.Items[KernelKey] as IKernel;
        }

        public static T Get<T>(this HttpContextBase context)
        {
            var kernel = GetKernel(context);
            return kernel == null ? default(T) : kernel.Get<T>();
        }

        public static void SetKernel(this HttpContextBase context, IKernel kernel)
        {
            context.Items[KernelKey] = kernel;
        }
    }
}