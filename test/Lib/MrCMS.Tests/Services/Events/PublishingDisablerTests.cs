using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Services;
using Xunit;

namespace MrCMS.Tests.Services.Events
{
    public class PublishingDisablerTests
    {
        public PublishingDisablerTests()
        {
            TypeHelper.Initialize(GetType().Assembly);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(typeof(TestEventImplementation));
            serviceCollection.AddTransient(typeof(ITestEvent), typeof(TestEventImplementation));
            serviceCollection.AddTransient(typeof(TestEventImplementation2));
            serviceCollection.AddTransient(typeof(ITestEvent), typeof(TestEventImplementation2));
            _eventContext =
                new EventContext(serviceCollection.BuildServiceProvider()
                    //new List<IEvent> {new TestEventImplementation(), new TestEventImplementation2()}
                );
            TestEventImplementation.Reset();
            TestEventImplementation2.Reset();
        }

        private EventContext _eventContext;

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
            using (_eventContext.Disable(typeof(ITestEvent)))
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