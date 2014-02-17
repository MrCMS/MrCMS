using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Galleries
{
    public class GalleriesApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Galleries"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }
    }
}