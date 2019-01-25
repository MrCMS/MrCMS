using System.Threading.Tasks;

namespace MrCMS.Helpers
{
    public static class AsyncHelper
    {
        public static T ExecuteSync<T>(this Task<T> task)
        {
            return task.GetAwaiter().GetResult();
        }
        public static void ExecuteSync(this Task task)
        {
            task.GetAwaiter().GetResult();
        }
    }
}