using System.IO;
using System.Web;

namespace MrCMS.Website
{
    public class OutOfContextResponse : HttpResponseBase
    {
        private readonly HttpCookieCollection _cookies;
        private MemoryStream _memoryStream;

        public OutOfContextResponse(HttpCookieCollection cookies)
        {
            _cookies = cookies;
            _memoryStream = new MemoryStream();
            Output = new StreamWriter(_memoryStream);
        }

        public override void Clear()
        {
        }
        public override void End()
        {
        }
        public override int StatusCode { get; set; }
        public override HttpCookieCollection Cookies
        {
            get { return _cookies; }
        }

        public override TextWriter Output { get; set; }

        public override Stream OutputStream
        {
            get { return _memoryStream; }
        }
    }
}