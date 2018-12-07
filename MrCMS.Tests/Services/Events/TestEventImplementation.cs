namespace MrCMS.Tests.ACL
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