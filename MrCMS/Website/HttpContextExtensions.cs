using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;

namespace MrCMS.Website
{
    public static class HttpContextExtensions
    {
        private const string CurrentKernelKey = "current.kernel";

        public static IKernel GetKernel(this HttpContextBase context)
        {
            return context.Items[CurrentKernelKey] as IKernel;
        }

        public static void SetKernel(this HttpContextBase context, IKernel kernel)
        {
            context.Items[CurrentKernelKey] = kernel;
        }

        public static T Get<T>(this HttpContextBase context)
        {
            var kernel = context.Items[CurrentKernelKey] as IKernel;
            return kernel != null ? kernel.Get<T>() : default(T);
        }

        public static IEnumerable<T> GetAll<T>(this HttpContextBase context)
        {
            var kernel = context.Items[CurrentKernelKey] as IKernel;
            return kernel != null ? kernel.GetAll<T>() : Enumerable.Empty<T>();
        }
    }
}