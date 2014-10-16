using FluentAssertions;
using MrCMS.Tests.ACL;
using Xunit;

namespace MrCMS.Tests.Services.Events
{
    public class PublishingDisablerTests : InMemoryDatabaseTest
    {
        public PublishingDisablerTests()
        {
            TestEventImplementation.Reset();
            TestEventImplementation2.Reset();
            EventContext.FakeNonCoreEvents = false;
        }

        [Fact]
        public void EnsureTheTestEventIsBehavingProperly()
        {
            EventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());

            TestEventImplementation.ExecutionCount.Should().Be(1);
        }

        [Fact]
        public void EnsureTheTestEventIsBehavingProperlyForMany()
        {
            EventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            EventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());

            TestEventImplementation.ExecutionCount.Should().Be(2);
        }

        [Fact]
        public void DisablingTheEventShouldPreventItBeingExecuted()
        {
            using (EventContext.Disable<ITestEvent>())
            {
                EventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            }
            TestEventImplementation.ExecutionCount.Should().Be(0);
        }

        [Fact]
        public void NonGenericDisablingTheEventShouldPreventItBeingExecuted()
        {
            using (EventContext.Disable(typeof (ITestEvent)))
            {
                EventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            }
            TestEventImplementation.ExecutionCount.Should().Be(0);
        }

        [Fact]
        public void DisablingAnImplementationShouldOnlyDisableThatImplementation()
        {
            using (EventContext.Disable<TestEventImplementation>())
            {
                EventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            }
            TestEventImplementation.ExecutionCount.Should().Be(0);
            TestEventImplementation2.ExecutionCount.Should().Be(1);
        }
    }
}