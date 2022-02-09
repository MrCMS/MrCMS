using System;
using System.Threading.Tasks;
using MrCMS.Tasks;

namespace MrCMS.Scheduling
{
    public interface IAdHocJobScheduler
    {
        Task Schedule<TJob>() where TJob : SchedulableTask;
        Task Schedule(Type type);
    }
}