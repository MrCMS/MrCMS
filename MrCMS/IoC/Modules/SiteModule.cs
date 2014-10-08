using MrCMS.Entities.Multisite;
using MrCMS.Website;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MrCMS.IoC.Modules
{
    public class SiteModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Rebind<Site>()
                .ToMethod(context => CurrentRequestData.CurrentSite)
                .InRequestScope();
        }
    }
}