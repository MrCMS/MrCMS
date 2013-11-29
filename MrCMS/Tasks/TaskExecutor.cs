using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Website;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Tasks
{
    public static class TaskExecutor
    {
        private static readonly ThreadLocal<List<BackgroundTask>> tasksToExecute =
            new ThreadLocal<List<BackgroundTask>>(() => new List<BackgroundTask>());

        public static ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
            set
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = value;
                }
            }
        }

        private static ISessionFactory _sessionFactory;

        public static Action<Exception> ExceptionHandler { get; set; }

        public static ThreadLocal<List<BackgroundTask>> TasksToExecute
        {
            get { return tasksToExecute; }
        }

        public static void ExecuteLater(BackgroundTask task)
        {
            TasksToExecute.Value.Add(task);
        }

        public static void Discard()
        {
            TasksToExecute.Value.Clear();
        }

        public static void StartExecuting()
        {
            var value = TasksToExecute.Value;
            var copy = value.ToArray();
            value.Clear();

            if (copy.Length > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    foreach (var backgroundTask in copy)
                    {
                        ExecuteTask(backgroundTask);
                    }
                }, TaskCreationOptions.LongRunning)
                    .ContinueWith(task =>
                    {
                        if (ExceptionHandler != null) ExceptionHandler(task.Exception);
                    }, TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        public static void ExecuteTask(BackgroundTask task)
        {
            using (var session = _sessionFactory.OpenFilteredSession())
            {
                CurrentRequestData.CurrentSite = session.Get<Site>(task.Site.Id);
                task.Run(session);
            }
        }
    }
}