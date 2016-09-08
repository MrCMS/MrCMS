using System.Collections;
using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using Ninject;

namespace MrCMS.Tests.DbConfiguration
{
    public class TestListenerApp : MrCMSApp
    {
        private static TestListener _testListener = new TestListener();

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }

        public override string AppName
        {
            get { return "TestListener"; }
        }

        public override string Version
        {
            get { return "1.0.0.0"; }
        }

        public static TestListener TestListener
        {
            get { return _testListener; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
        }

        protected override void AppendConfiguration(Configuration configuration)
        {
            TestListener.Reset();
            _testListener = new TestListener();
            configuration.AppendListeners(ListenerType.PostCommitInsert, new IPostInsertEventListener[] { TestListener });
        }
    }

    public class TestListener : IPostInsertEventListener
    {
        private int _hitCount;
        public int HitCount { get { return _hitCount; } }
        public void OnPostInsert(PostInsertEvent @event)
        {
            _hitCount++;
        }

        public void Reset()
        {
            _hitCount = 0;
        }
    }
}