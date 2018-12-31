using System;
using System.Collections.Generic;
using FakeItEasy;
using FakeItEasy.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.TestSupport
{
    public abstract class MrCMSTest : IDisposable
    {
        protected TestableEventContext _eventContext = new TestableEventContext();
        private readonly ServiceCollection _serviceCollection = new ServiceCollection();
        protected TestableEventContext EventContext => _eventContext;


        protected MrCMSTest()
        {
            //ServiceCollection.Load(new TestContextModule());
            ServiceCollection.AddScoped<IHttpContextAccessor, DummyContextAccessor>();
            ServiceCollection.AddTransient<IEventContext>(provider => _eventContext);
            TypeHelper.Initialize(GetType().Assembly);
            //MrCMSKernel.OverrideKernel(ServiceCollection);
            //CurrentRequestData.SiteSettings = new SiteSettings();
        }

        public IEnumerable<ICompletedFakeObjectCall> EventsRaised => Fake.GetCalls(EventContext.FakeEventContext);

        public IServiceCollection ServiceCollection => _serviceCollection;
        public IServiceProvider ServiceProvider => _serviceCollection.BuildServiceProvider();

        public virtual void Dispose()
        {
        }
    }

    public class DummyContextAccessor : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; } = new DefaultHttpContext();
    }
}