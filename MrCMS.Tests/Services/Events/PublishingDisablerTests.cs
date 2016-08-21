using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Events;
using MrCMS.Services;
using MrCMS.Tests.ACL;
using Xunit;

namespace MrCMS.Tests.Services.Events
{
    public class PublishingDisablerTests 
    {
        public PublishingDisablerTests()
        {
            _eventContext =
                new TestableEventContext(
                    new EventContext(new List<IEvent> {new TestEventImplementation(), new TestEventImplementation2()}));
            TestEventImplementation.Reset();
            TestEventImplementation2.Reset();
            _eventContext.FakeNonCoreEvents = false;
        }

        private TestableEventContext _eventContext;

        [Fact]
        public void EnsureTheTestEventIsBehavingProperly()
        {
            _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());

            TestEventImplementation.ExecutionCount.Should().Be(1);
        }

        [Fact]
        public void EnsureTheTestEventIsBehavingProperlyForMany()
        {
            _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());

            TestEventImplementation.ExecutionCount.Should().Be(2);
        }

        [Fact]
        public void DisablingTheEventShouldPreventItBeingExecuted()
        {
            using (_eventContext.Disable<ITestEvent>())
            {
                _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            }
            TestEventImplementation.ExecutionCount.Should().Be(0);
        }

        [Fact]
        public void NonGenericDisablingTheEventShouldPreventItBeingExecuted()
        {
            using (_eventContext.Disable(typeof (ITestEvent)))
            {
                _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            }
            TestEventImplementation.ExecutionCount.Should().Be(0);
        }

        [Fact]
        public void DisablingAnImplementationShouldOnlyDisableThatImplementation()
        {
            using (_eventContext.Disable<TestEventImplementation>())
            {
                _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            }
            TestEventImplementation.ExecutionCount.Should().Be(0);
            TestEventImplementation2.ExecutionCount.Should().Be(1);
        }
    }
}