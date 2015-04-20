using System;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MrCMS.IoC.Modules;
using MrCMS.Website;
using Ninject;
using Ninject.Web.Common;
using WebActivatorEx;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MrCMSKernel), "Start", Order = 1)]
[assembly: ApplicationShutdownMethod(typeof(MrCMSKernel), "Stop")]

namespace MrCMS.Website
{
    public static class MrCMSKernel
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();
        private static IKernel _kernel;

        public static IKernel Kernel
        {
            get { return _kernel ?? Bootstrapper.Kernel; }
        }
        /// <summary>
        ///     Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        ///     Stops the application.
        /// </summary>
        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        /// <summary>
        ///     Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel(new NinjectSettings { AllowNullInjection = true },
                new ServiceModule(),
                new SettingsModule(),
                new FileSystemModule(),
                new GenericBindingsModule(),
                new SystemWebModule(),
                new SiteModule(),
                new AuthModule());
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            kernel.Load(new NHibernateModule());
            return kernel;
        }

        public static void OverrideKernel(IKernel kernel)
        {
            _kernel = kernel;
        }
    }
}