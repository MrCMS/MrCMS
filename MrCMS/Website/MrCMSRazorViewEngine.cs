using System.Web;
using System.Web.Mvc;

namespace MrCMS.Website
{
    public class MrCMSRazorViewEngine : MrCMSViewEngine
    {
        internal static readonly string ViewStartFileName = "_ViewStart";

        public MrCMSRazorViewEngine()
            : this(null)
        {
        }

        public MrCMSRazorViewEngine(IViewPageActivator viewPageActivator)
        {
            ViewPageActivator = viewPageActivator;

            ViewLocationCache = HttpContext.Current == null
                ? DefaultViewLocationCache.Null
                : new DefaultViewLocationCache();
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return new RazorView(controllerContext, partialPath, null, false, ViewEngineFileLocations.FileExtensions,
                ViewPageActivator);
        }

        protected override bool FileExists(string virtualPath)
        {
            return VirtualPathProvider.FileExists(virtualPath);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var view = new RazorView(controllerContext, viewPath, masterPath, true,
                ViewEngineFileLocations.FileExtensions, ViewPageActivator);
            return view;
        }

    }
}