using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using MrCMS.Website;
using Ninject.Modules;

namespace MrCMS.IoC.Modules
{
    public class SystemWebModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<HttpRequestBase>().ToMethod(context => CurrentRequestData.CurrentContext.Request);
            Kernel.Bind<HttpResponseBase>().ToMethod(context => CurrentRequestData.CurrentContext.Response);
            Kernel.Bind<HttpSessionStateBase>().ToMethod(context => CurrentRequestData.CurrentContext.Session);
            Kernel.Bind<HttpServerUtilityBase>().ToMethod(context => CurrentRequestData.CurrentContext.Server);
            Kernel.Bind<ObjectCache>().ToMethod(context => MemoryCache.Default);
            Kernel.Bind<Cache>().ToMethod(context => CurrentRequestData.CurrentContext.Cache);
            Kernel.Bind<UrlHelper>()
                .ToMethod(context => new UrlHelper(CurrentRequestData.CurrentContext.Request.RequestContext));
        }
    }
}