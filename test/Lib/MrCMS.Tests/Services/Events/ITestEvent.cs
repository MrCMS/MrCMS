using MrCMS.Events;

namespace MrCMS.Tests.Services.Events
{
    public interface ITestEvent : IEvent<TestEventArgs>
    {
    }
}