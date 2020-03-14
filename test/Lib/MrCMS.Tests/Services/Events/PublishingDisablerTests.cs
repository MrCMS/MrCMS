using System.Threading.Tasks;
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

        private readonly EventContext _eventContext;

        [Fact]
        public async Task EnsureTheTestEventIsBehavingProperly()
        {
            await _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());

            TestEventImplementation.ExecutionCount.Should().Be(1);
        }

        [Fact]
        public async Task EnsureTheTestEventIsBehavingProperlyForMany()
        {
            await _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            await _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());

            TestEventImplementation.ExecutionCount.Should().Be(2);
        }

        [Fact]
        public async Task DisablingTheEventShouldPreventItBeingExecuted()
        {
            using (_eventContext.Disable<ITestEvent>())
            {
                await _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            }
            TestEventImplementation.ExecutionCount.Should().Be(0);
        }

        [Fact]
        public async Task NonGenericDisablingTheEventShouldPreventItBeingExecuted()
        {
            using (_eventContext.Disable(typeof(ITestEvent)))
            {
                await _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            }
            TestEventImplementation.ExecutionCount.Should().Be(0);
        }

        [Fact]
        public async Task DisablingAnImplementationShouldOnlyDisableThatImplementation()
        {
            using (_eventContext.Disable<TestEventImplementation>())
            {
                await _eventContext.Publish<ITestEvent, TestEventArgs>(new TestEventArgs());
            }
            TestEventImplementation.ExecutionCount.Should().Be(0);
            TestEventImplementation2.ExecutionCount.Should().Be(1);
        }
    }
}