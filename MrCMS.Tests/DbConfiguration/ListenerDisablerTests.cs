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
        [Fact]
        public void WithoutTheListenerDisabledListenersShouldBeTriggered()
        {
            TestListener.Reset();
            Session.Transact(session => session.Save(new BasicMappedWebpage()));
            TestListener.HitCount.Should().Be(1);
        }

        [Fact]
        public void WithTheListenerDisablerListenersShouldNotBeTriggered()
        {
            TestListener.Reset();
            using (new ListenerDisabler(Session))
            {
                Session.Transact(session => session.Save(new BasicMappedWebpage()));
            }
            TestListener.HitCount.Should().Be(0);
        }

        [Fact]
        public void AfterTheScopeOfTheDisablerListenersShouldBeTriggeredAgain()
        {
            TestListener.Reset();
            using (new ListenerDisabler(Session))
            {
                Session.Transact(session => session.Save(new BasicMappedWebpage()));
            }
            Session.Transact(session => session.Save(new BasicMappedWebpage()));
            TestListener.HitCount.Should().Be(1);
        }
    }

    public class TestListenerApp : MrCMSApp
    {
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

        protected override void RegisterServices(IKernel kernel)
        {
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
        }

        protected override void AppendConfiguration(Configuration configuration)
        {
            TestListener.Reset();
            configuration.AppendListeners(ListenerType.PostCommitInsert, new IPostInsertEventListener[] { new TestListener() });
        }
    }

    public class TestListener : IPostInsertEventListener
    {
        private static int _hitCount;
        public static int HitCount { get { return _hitCount; } }
        public void OnPostInsert(PostInsertEvent @event)
        {
            _hitCount++;
        }

        public static void Reset()
        {
            _hitCount = 0;
        }
    }
}