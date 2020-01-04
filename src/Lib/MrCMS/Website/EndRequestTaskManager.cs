using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Website
{
    public class EndRequestTaskManager : IEndRequestTaskManager
    {
        private const string Key = "current.on-end-request";
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOnEndRequestExecutor _onEndRequestExecutor;

        public EndRequestTaskManager(IHttpContextAccessor contextAccessor, IOnEndRequestExecutor onEndRequestExecutor)
        {
            _contextAccessor = contextAccessor;
            _onEndRequestExecutor = onEndRequestExecutor;
        }

        public void AddTask(EndRequestTask task)
        {
            GetTasks().Add(task);
        }

        public Task ExecuteTasks(CancellationToken token)
        {
            return _onEndRequestExecutor.ExecuteTasks(GetTasks(), token);
        }

        public HashSet<EndRequestTask> GetTasks()
        {
            return (HashSet<EndRequestTask>)(_contextAccessor.HttpContext.Items[Key] ??
                                              (_contextAccessor.HttpContext.Items[Key] =
                                                  new HashSet<EndRequestTask>()));
        }
    }
}