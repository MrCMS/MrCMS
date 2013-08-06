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
        private readonly OutOfContextResponse _outOfContextResponse = new OutOfContextResponse();

        public override IDictionary Items { get { return _items; } }

        public override HttpServerUtilityBase Server { get { return _outOfContextServerUtility; } }

        public override HttpSessionStateBase Session { get { return _outOfContextSession; } }

        public override HttpResponseBase Response { get { return _outOfContextResponse; } }
    }
}