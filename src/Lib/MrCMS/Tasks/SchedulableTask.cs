using System;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public abstract class SchedulableTask
    {
        public abstract int Priority { get; }

        public async Task Execute(CancellationToken token) => await OnExecute(token);
        protected abstract Task OnExecute(CancellationToken token);
    }
}