using MrCMS.Apps;
using Ninject;

namespace MrCMS.Web.Apps.Galleries
{
    public class GalleriesApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Galleries"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }
    }
}