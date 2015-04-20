using System;
using System.Collections.Generic;
using FakeItEasy;
using FakeItEasy.Core;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;
using Ninject;
using Ninject.MockingKernel;

namespace MrCMS.Tests
{
    public abstract class MrCMSTest : IDisposable
    {
        protected TestableEventContext _eventContext = new TestableEventContext();
        protected TestableEventContext EventContext { get { return _eventContext; } }

        private readonly MockingKernel _kernel;

        protected MrCMSTest()
        {
            _kernel = new MockingKernel();
            Kernel.Load(new TestContextModule());
            Kernel.Bind<IEventContext>().ToMethod(context => _eventContext);
            MrCMSKernel.OverrideKernel(Kernel);
            CurrentRequestData.SiteSettings = new SiteSettings();
        }

        public IEnumerable<ICompletedFakeObjectCall> EventsRaised
        {
            get { return Fake.GetCalls(EventContext.FakeEventContext); }
        }

        public MockingKernel Kernel
        {
            get { return _kernel; }
        }

        public virtual void Dispose()
        {
        }
    }
}