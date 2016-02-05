using HibernatingRhinos.Profiler.Appender.NHibernate;
using MrCMS.Website;

namespace MrCMS.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : MrCMSApplication
    {
        protected override void OnApplicationStart()
        {
            NHibernateProfiler.Initialize();
        }
    }
}