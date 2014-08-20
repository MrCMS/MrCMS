using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using Ninject;

namespace MrCMS.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : MrCMSApplication
    {
        public override string RootNamespace
        {
            get { return typeof(WidgetController).Namespace; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
        }
    }
}