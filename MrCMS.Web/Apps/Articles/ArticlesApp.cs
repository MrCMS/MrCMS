using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.Articles
{
    public class ArticlesApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Articles"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            throw new System.NotImplementedException();
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }
    }
}