using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrCMS.IoC;
using MrCMS.Settings;
using MrCMS.Website;
using Ninject;
using Ninject.MockingKernel;

namespace MrCMS.Web.Tests
{
    public abstract class MrCMSTest : IDisposable
    {
        protected MrCMSTest()
        {
            var mockingKernel = new MockingKernel();
            mockingKernel.Load(new ContextModule());
            MrCMSApplication.OverrideKernel(mockingKernel);
            CurrentRequestData.SiteSettings = new SiteSettings();
        }

        public virtual void Dispose()
        {
        }
    }
}
