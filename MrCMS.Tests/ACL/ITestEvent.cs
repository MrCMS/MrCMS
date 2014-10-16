using MrCMS.Events;

namespace MrCMS.Tests.ACL
{
    public interface ITestEvent : IEvent<TestEventArgs>
    {
    }
}