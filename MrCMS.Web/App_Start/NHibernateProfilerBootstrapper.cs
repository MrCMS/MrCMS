using HibernatingRhinos.Profiler.Appender.NHibernate;

[assembly: WebActivator.PreApplicationStartMethod(typeof(MrCMS.Web.App_Start.NHibernateProfilerBootstrapper), "PreStart")]
namespace MrCMS.Web.App_Start
{
	public static class NHibernateProfilerBootstrapper
	{
		public static void PreStart()
		{
			// Initialize the profiler
			NHibernateProfiler.Initialize();
			
			// You can also use the profiler in an offline manner.
			// This will generate a file with a snapshot of all the NHibernate activity in the application,
			// which you can use for later analysis by loading the file into the profiler.
			// var filename = @"c:\profiler-log";
			// NHibernateProfiler.InitializeOfflineProfiling(filename);
		}
	}
}

