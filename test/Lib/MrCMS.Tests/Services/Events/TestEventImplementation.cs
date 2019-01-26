namespace MrCMS.Tests.Services.Events
{
    public class TestEventImplementation : ITestEvent
    {
        public static int ExecutionCount = 0;

        public void Execute(TestEventArgs args)
        {
            ExecutionCount++;
        }

        public static void Reset()
        {
            ExecutionCount = 0;
        }
    }
}