using System;
using MrCMS.IoC;
using MrCMS.Settings;
using MrCMS.Website;
using Ninject;
using Ninject.MockingKernel;

namespace MrCMS.Commenting.Tests
{
    public abstract class MrCMSTest : IDisposable
    {
        protected readonly MockingKernel Kernel;

        protected MrCMSTest()
        {
            Kernel = new MockingKernel();
            Kernel.Load(new ContextModule(), new MrCMSMockModule());
            MrCMSApplication.OverrideKernel(Kernel);
            CurrentRequestData.SiteSettings = new SiteSettings();
        }

        public virtual void Dispose()
        {
        }
    }
}