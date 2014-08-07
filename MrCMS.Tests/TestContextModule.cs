using System.Web;
using MrCMS.Website;
using Ninject.Modules;

namespace MrCMS.Tests
{
    public class TestContextModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<HttpContextBase>().To<OutOfContext>().InThreadScope();
        }
    }
}