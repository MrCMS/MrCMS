using System.IO;
using System.Web.Routing;

namespace MrCMS.Website.Routing
{
    public class FileNotFoundHandler : IMrCMSRouteHandler
    {
        public bool Handle(RequestContext context)
        {
            var path = context.HttpContext.Request.Url.AbsolutePath;
            var extension = Path.GetExtension(path);
            if (!string.IsNullOrWhiteSpace(extension))
            {
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.StatusCode = 404;
                context.HttpContext.Response.End();
                return true;
            }
            return false;
        }

        public int Priority { get { return 10; } }
    }
}