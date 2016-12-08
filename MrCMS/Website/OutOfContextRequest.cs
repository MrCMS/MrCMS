using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace MrCMS.Website
{
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

        public void SetIsLocal(bool isLocal)
        {
            _isLocal = isLocal;
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