using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

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

        public void ExecuteTasks()
        {
            _onEndRequestExecutor.ExecuteTasks(GetTasks());
        }

        public HashSet<EndRequestTask> GetTasks()
        {
            return (HashSet<EndRequestTask>)(_contextAccessor.HttpContext.Items[Key] ??
                                              (_contextAccessor.HttpContext.Items[Key] =
                                                  new HashSet<EndRequestTask>()));
        }
    }
}