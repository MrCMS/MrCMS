using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace MrCMS.Website
{
    public class OutOfContext : HttpContextBase
    {
        private readonly IDictionary _items = new Dictionary<string, object>();
        private readonly OutOfContextServerUtility _outOfContextServerUtility = new OutOfContextServerUtility();
        private readonly OutOfContextSession _outOfContextSession = new OutOfContextSession();
        private readonly HttpCookieCollection _cookies = new HttpCookieCollection();

        private readonly OutOfContextResponse _outOfContextResponse;
        private readonly OutOfContextRequest _outOfContextRequest;
        public OutOfContext()
        {
            _outOfContextRequest = new OutOfContextRequest(_cookies);
            _outOfContextResponse = new OutOfContextResponse(_cookies);
        }
        public override IDictionary Items { get { return _items; } }

        public override HttpServerUtilityBase Server { get { return _outOfContextServerUtility; } }

        public override HttpSessionStateBase Session { get { return _outOfContextSession; } }

        public override HttpResponseBase Response { get { return _outOfContextResponse; } }

        public override HttpRequestBase Request
        {
            get { return _outOfContextRequest; }
        }
    }


    public class OutOfContextRequest : HttpRequestBase
    {
        private readonly HttpCookieCollection _cookies;

        public OutOfContextRequest(HttpCookieCollection cookies)
        {
            _cookies = cookies;
        }

        public override Uri Url
        {
            get { return new Uri("http://www.example.com"); }
        }

        public override HttpCookieCollection Cookies
        {
            get { return _cookies; }
        }
    }
}