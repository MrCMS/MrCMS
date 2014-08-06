using System.Collections;
using FluentAssertions;
using MrCMS.Apps;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using Ninject;
using Xunit;

namespace MrCMS.Tests.DbConfiguration
{
    public class ListenerDisablerTests : InMemoryDatabaseTest
    {
        [Fact(Skip = "Refactored listeners")]
        public void WithoutTheListenerDisabledListenersShouldBeTriggered()
        {
            TestListenerApp.TestListener.Reset();
            Session.Transact(session => session.Save(new BasicMappedWebpage()));
            TestListenerApp.TestListener.HitCount.Should().Be(1);
        }

        [Fact]
        public void WithTheListenerDisablerListenersShouldNotBeTriggered()
        {
            TestListenerApp.TestListener.Reset();
            using (new ListenerDisabler(Session, ListenerType.PostCommitInsert, TestListenerApp.TestListener))
            {
                Session.Transact(session => session.Save(new BasicMappedWebpage()));
            }
            TestListenerApp.TestListener.HitCount.Should().Be(0);
        }

        [Fact(Skip = "Refactored listeners")]
        public void AfterTheScopeOfTheDisablerListenersShouldBeTriggeredAgain()
        {
            TestListenerApp.TestListener.Reset();
            using (new ListenerDisabler(Session, ListenerType.PostCommitInsert, TestListenerApp.TestListener))
            {
                Session.Transact(session => session.Save(new BasicMappedWebpage()));
            }
            Session.Transact(session => session.Save(new BasicMappedWebpage()));
            TestListenerApp.TestListener.HitCount.Should().Be(1);
        }
    }

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

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
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