using MrCMS.Apps;
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

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }
    }
}