using System.Threading.Tasks;

namespace MrCMS.Tests.Services.Events
{
    public class TestEventImplementation2 : ITestEvent
    {
        public static int ExecutionCount = 0;

        public Task Execute(TestEventArgs args)
        {
            ExecutionCount++;
        }

        public static void Reset()
        {
            ExecutionCount = 0;
        }
    }
}