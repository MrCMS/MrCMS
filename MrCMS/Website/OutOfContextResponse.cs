using System.Web;

namespace MrCMS.Website
{
    public class OutOfContextResponse : HttpResponseBase
    {
        public override void Clear()
        {
        }
        public override void End()
        {
        }
        public override int StatusCode { get; set; }
    }
}