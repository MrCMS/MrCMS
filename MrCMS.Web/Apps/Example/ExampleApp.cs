using MrCMS.Apps;
using Ninject;

namespace MrCMS.Web.Apps.Example
{
    public class ExampleApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Example"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }
    }
}