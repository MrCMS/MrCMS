using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Instrumentation;
using Elmah.ContentSyndication;
using UnvalidatedRequestValues = System.Web.Helpers.UnvalidatedRequestValues;

namespace MrCMS.Website
{
    public class OutOfContext : HttpContextBase
    {
        private readonly IDictionary _items = new Dictionary<object, object>();
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

        public override IPrincipal User { get; set; }

        public override HttpRequestBase Request
        {
            get { return _outOfContextRequest; }
        }

        public override PageInstrumentationService PageInstrumentation
        {
            get { return new PageInstrumentationService(); }
        }
    }


    public class OutOfContextRequest : HttpRequestBase
    {
        private readonly HttpCookieCollection _cookies;
        private NameValueCollection _form = new NameValueCollection();
        private Dictionary<string, string> _items = new Dictionary<string, string>();
        private bool _isLocal;

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

        public override bool IsLocal
        {
            get { return _isLocal; }
        }

        public override void ValidateInput()
        {

        }

        public override string UserAgent
        {
            get { return string.Empty; }
        }



        public override UnvalidatedRequestValuesBase Unvalidated
        {
            get { return null; }
        }

        public override NameValueCollection Form
        {
            get { return _form; }
        }

        public override string this[string key]
        {
            get { return _items.ContainsKey(key) ? _items[key] : null; }
        }
    }
}