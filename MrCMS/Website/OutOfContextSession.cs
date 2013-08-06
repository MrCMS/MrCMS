using System.Collections.Generic;
using System.Web;

namespace MrCMS.Website
{
    public class OutOfContextSession : HttpSessionStateBase
    {
        private readonly Dictionary<string, object> _dictionary;

        public OutOfContextSession()
        {
            _dictionary = new Dictionary<string, object>();
        }

        public override void Add(string name, object value)
        {
            _dictionary.Add(name, value);
        }

        public override void Clear()
        {
            _dictionary.Clear();
        }

        public override int Count
        {
            get { return _dictionary.Count; }
        }

        public override void Remove(string name)
        {
            _dictionary.Remove(name);
        }

        public override object this[string name]
        {
            get { return _dictionary.ContainsKey(name) ? _dictionary[name] : null; }
            set { _dictionary[name] = value; }
        }
    }
}