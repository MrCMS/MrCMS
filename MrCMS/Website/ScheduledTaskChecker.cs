using System.Timers;
using MrCMS.Tasks;

namespace MrCMS.Website
{
    public class ScheduledTaskChecker
    {
        private static readonly object obj = new object();
        private static ScheduledTaskChecker _instance;
        private Timer _timer;

        public static ScheduledTaskChecker Instance
        {
            get
            {
                lock (obj)
                {
                    return _instance ?? (_instance = new ScheduledTaskChecker());
                }
            }
        }

        public void Start(int checkEveryXSeconds)
        {
            _timer = new Timer
                         {
                             Interval = checkEveryXSeconds*1000
                         };
            _timer.Elapsed += AppendScheduledTasks;
            _timer.Start();
        }

        private void AppendScheduledTasks(object sender, ElapsedEventArgs e)
        {
            var scheduledTaskManager = MrCMSApplication.Get<IScheduledTaskManager>();
            foreach (var scheduledTask in scheduledTaskManager.GetDueTasks())
                TaskExecutor.ExecuteLater(scheduledTaskManager.GetTask(scheduledTask));

            TaskExecutor.StartExecuting();
        }
    }
}