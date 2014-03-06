using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MrCMS.IoC;
using MrCMS.Settings;
using MrCMS.Website;
using Ninject;
using Ninject.MockingKernel;
using Ninject.Modules;

namespace MrCMS.Web.Tests
{
    public abstract class MrCMSTest : IDisposable
    {
        protected MrCMSTest()
        {
            var mockingKernel = new MockingKernel();
            mockingKernel.Load(new TestContextModule());
            MrCMSApplication.OverrideKernel(mockingKernel);
            CurrentRequestData.SiteSettings = new SiteSettings();
        }

        public virtual void Dispose()
        {
        }
    }

    public class TestContextModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<HttpContextBase>().To<OutOfContext>().InThreadScope();
        }
    }
}
