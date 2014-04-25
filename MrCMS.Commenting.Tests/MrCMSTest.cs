using System;
using System.Web;
using MrCMS.IoC;
using MrCMS.Settings;
using MrCMS.Website;
using Ninject;
using Ninject.MockingKernel;
using Ninject.Modules;

namespace MrCMS.Commenting.Tests
{
    public class TestContextModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<HttpContextBase>().To<OutOfContext>().InThreadScope();
        }
    }
    public abstract class MrCMSTest : IDisposable
    {
        protected readonly MockingKernel Kernel;

        protected MrCMSTest()
        {
            Kernel = new MockingKernel();
            Kernel.Load(new TestContextModule(), new MrCMSMockModule());
            MrCMSApplication.OverrideKernel(Kernel);
            CurrentRequestData.SiteSettings = new SiteSettings();
        }

        public virtual void Dispose()
        {
        }
    }
}