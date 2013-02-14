using MrCMS.Apps;

namespace MrCMS.Web.Apps.Example
{
    public class ExampleApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Example"; }
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }
    }
}