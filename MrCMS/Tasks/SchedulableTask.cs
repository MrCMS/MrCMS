using System;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public abstract class SchedulableTask 
    {
        public abstract int Priority { get; }

        public Exception Execute()
        {
            try
            {
                OnExecute();
                return null;
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                return ex;
            }
        }
        protected abstract void OnExecute();
    }
}