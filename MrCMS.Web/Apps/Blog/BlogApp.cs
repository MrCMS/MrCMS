using System.Reflection;
using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Web.Apps.Blog.Controllers;

namespace MrCMS.Web.Apps.Blog
{
    public class BlogApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Blog"; }
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            //context.MapAreaRoute(
            //    "Admin_blog", "admin",
            //    "admin/blog/{controller}/{action}/{id}",
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new string[] { typeof(Areas.Admin.Controllers.PostController).Namespace }
            //    );
            context.MapRoute(
                "blog routes",
                "apps/blog/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },new string[]{typeof(PostController).Namespace}
                );
        }
    }
}