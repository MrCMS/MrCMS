using System;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public abstract class SchedulableTask
    {
        public abstract int Priority { get; }

        public void Execute()
        {
            OnExecute();
        }
        protected abstract void OnExecute();
    }
}