//using System.Threading.Tasks;
//using System.Xml;
//using Microsoft.AspNetCore.Mvc;

//namespace MrCMS.Web.Apps.Articles.ActionResults
//{
//    public class RssActionResult : ActionResult
//    {
//        public SyndicationFeed Feed { get; set; }

//        public override void ExecuteResult(ActionContext context)
//        {
//            context.HttpContext.Response.ContentType = "application/rss+xml";

//            Rss20FeedFormatter rssFormatter = new Rss20FeedFormatter(Feed);
//            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output))
//            {
//                rssFormatter.WriteTo(writer);
//            }
//        }

//        public override Task ExecuteResultAsync(ActionContext context)
//        {
//            return base.ExecuteResultAsync(context);
//        }
//    }
//}
