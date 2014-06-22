using System.Web;

namespace MrCMS.Website.Routing
{
    public class NotInstalledHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Redirect("~/Install");
        }

        public bool IsReusable { get; private set; }
    }
}