Imports HibernatingRhinos.Profiler.Appender.NHibernate

<assembly: WebActivator.PreApplicationStartMethod(GetType(Global.MrCMS.Web.App_Start.NHibernateProfilerBootstrapper), "PreStart")>
Namespace App_Start
	Public Class NHibernateProfilerBootstrapper
        Public Shared Sub PreStart()
            ' Initialize the profiler
			NHibernateProfiler.Initialize()

            ' You can also use the profiler in an offline manner.
            ' This will generate a file with a snapshot of all the NHibernate activity in the application,
            ' which you can use for later analysis by loading the file into the profiler.
            ' Dim FileName as String = @"c:\profiler-log";
            ' NHibernateProfiler.InitializeOfflineProfiling(FileName)
        End Sub
    End Class
End Namespace

